using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SurfaceSystem.OpenFracture;

[CreateAssetMenu(fileName = "GlassFractureEffect", menuName = "SurfaceSystem/Effects/Glass Fracture Effect")]
public class GlassFractureEffect : SurfaceEffect
{
    [Header("Impact")]
    [SerializeField, Min(0f)] private float _breakImpulseThreshold = 8f;

    [Header("Fragments")]
    [SerializeField, Range(2, 32)] private int _fragmentCount = 12;
    [SerializeField, Min(0.1f)] private float _totalMass = 10f;
    [SerializeField, Min(0f)] private float _fragmentSpeed = 2f;
    [SerializeField, Min(0.1f)] private float _fragmentLifetime = 8f;
    [SerializeField] private bool _useWorldGravity;
    [SerializeField] private int _randomSeed = 12345;
    [SerializeField] private Material _insideMaterial;

    public float FragmentLifetime { get { return _fragmentLifetime; } }

    // 유리 파괴 기준을 플레이어와 공통 설정 조회에 제공한다.
    public override void Modify(ref SurfaceModifiers modifiers, float normalizedSpeed)
    {
        modifiers.breakEnabled = true;
        modifiers.breakImpulseThreshold = _breakImpulseThreshold;
    }

    // 실제 충격량을 판정하고 충돌한 행성에서 파괴 작업을 시작한다.
    public override void OnImpact(SurfaceInstance surface, Collision collision, SurfaceModifiers modifiers)
    {
        if (!modifiers.breakEnabled || collision.contactCount == 0
            || collision.impulse.magnitude < modifiers.breakImpulseThreshold)
        {
            return;
        }

        if (collision.gameObject.GetComponentInParent<SurfaceAgent>() == null)
        {
            return;
        }

        MeshGlass glass = surface.GetComponent<MeshGlass>();
        if (glass == null)
        {
            Debug.LogError("GlassFractureEffect requires MeshGlass on the SurfaceInstance object.", surface);
            return;
        }

        glass.BeginFracture(this, collision.GetContact(0).point);
    }

    // OpenFracture로 실제 메시를 절단하며 한 프레임에 한 번씩 분할한다.
    public IEnumerator Fracture(MeshGlass glass, Vector3 impactPoint)
    {
        Mesh source = glass.GetComponent<MeshFilter>().sharedMesh;
        if (source == null || !source.isReadable || source.subMeshCount != 1)
        {
            throw new InvalidOperationException("Glass requires a readable, closed mesh with one submesh.");
        }

        if (glass.GetComponentsInChildren<Collider>().Length != glass.GetComponents<Collider>().Length)
        {
            throw new InvalidOperationException("Place Glass colliders on the same object as MeshGlass.");
        }

        Mesh worldMesh = Instantiate(source);
        glass.TrackMesh(worldMesh);
        Vector3[] vertices = worldMesh.vertices;
        Vector3[] normals = worldMesh.normals;
        if (normals.Length != vertices.Length)
        {
            worldMesh.RecalculateNormals();
            normals = worldMesh.normals;
        }

        Matrix4x4 matrix = glass.transform.localToWorldMatrix;
        if (matrix.determinant <= 0f)
        {
            throw new InvalidOperationException("Glass requires a positive, non-zero transform scale.");
        }

        Matrix4x4 normalMatrix = matrix.inverse.transpose;
        Vector3 origin = glass.transform.position;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = matrix.MultiplyPoint3x4(vertices[i]) - origin;
            normals[i] = normalMatrix.MultiplyVector(normals[i]).normalized;
        }

        worldMesh.vertices = vertices;
        worldMesh.normals = normals;
        if (worldMesh.uv.Length != vertices.Length)
        {
            worldMesh.uv = new Vector2[vertices.Length];
        }

        List<FragmentData> fragments = new List<FragmentData>();
        fragments.Add(new FragmentData(worldMesh));
        System.Random random = new System.Random(_randomSeed);
        int targetCount = Mathf.Clamp(_fragmentCount, 2, 32);
        while (fragments.Count < targetCount)
        {
            int largest = FindLargestFragment(fragments);
            FragmentData fragment = fragments[largest];
            Vector3 normal = new Vector3((float)random.NextDouble() - 0.5f,
                (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f).normalized;
            MeshSlicer.Slice(fragment, normal, fragment.Bounds.center, Vector2.one, Vector2.zero,
                out FragmentData first, out FragmentData second);

            if (first.triangleCount < 12 || second.triangleCount < 12
                || first.Triangles[1].Count == 0 || second.Triangles[1].Count == 0)
            {
                throw new InvalidOperationException("Fracture produced an invalid cut. Check mesh topology or change Random Seed.");
            }

            first.CalculateBounds();
            second.CalculateBounds();
            fragments[largest] = first;
            fragments.Add(second);
            yield return null;
        }

        GameObject root = glass.CreateFragmentRoot();
        Material exterior = glass.GetComponent<MeshRenderer>().sharedMaterial;
        Material interior = _insideMaterial;
        if (interior == null)
        {
            interior = exterior;
        }

        PhysicsMaterial physicsMaterial = glass.GetComponent<SurfaceInstance>().Profile.PhysicsMaterial;
        Rigidbody sourceBody = glass.GetComponent<Rigidbody>();
        foreach (FragmentData fragment in fragments)
        {
            Mesh mesh = fragment.ToMesh();
            glass.TrackMesh(mesh);
            Vector3 center = mesh.bounds.center;
            Vector3[] positions = mesh.vertices;
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] -= center;
            }

            mesh.vertices = positions;
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            GameObject shard = new GameObject("Glass Fragment " + root.transform.childCount);
            shard.layer = glass.gameObject.layer;
            shard.transform.SetParent(root.transform, false);
            shard.transform.position = origin + center;
            shard.AddComponent<MeshFilter>().sharedMesh = mesh;
            shard.AddComponent<MeshRenderer>().sharedMaterials = new Material[] { exterior, interior };
            MeshCollider collider = shard.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = mesh;
            collider.sharedMaterial = physicsMaterial;
            Rigidbody body = shard.AddComponent<Rigidbody>();
            body.mass = Mathf.Max(0.001f, _totalMass / fragments.Count);
            body.useGravity = _useWorldGravity;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            Vector3 velocity = (shard.transform.position - impactPoint).normalized * _fragmentSpeed;
            if (sourceBody != null)
            {
                velocity += sourceBody.GetPointVelocity(shard.transform.position);
            }

            body.linearVelocity = velocity;
            yield return null;
        }

        glass.CompleteFracture();
    }

    // 큰 파편부터 나눠 지나치게 작은 조각이 생기는 것을 줄인다.
    private int FindLargestFragment(List<FragmentData> fragments)
    {
        int largest = 0;
        float largestVolume = -1f;
        for (int i = 0; i < fragments.Count; i++)
        {
            Vector3 size = fragments[i].Bounds.size;
            float volume = size.x * size.y * size.z;
            if (volume > largestVolume)
            {
                largest = i;
                largestVolume = volume;
            }
        }

        return largest;
    }
}
