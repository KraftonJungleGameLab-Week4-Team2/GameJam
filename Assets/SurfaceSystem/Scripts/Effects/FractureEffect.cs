using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SurfaceSystem.OpenFracture;

[CreateAssetMenu(fileName = "FractureEffect", menuName = "SurfaceSystem/Effects/Fracture Effect")]
public class FractureEffect : SurfaceEffect
{
    [Header("Fragments")]
    [SerializeField, Range(2, 32)] private int _fragmentCount = 12;
    [SerializeField, Min(0.1f)] private float _totalMass = 10f;
    [SerializeField, Min(0f)] private float _fragmentSpeed = 2f;
    [SerializeField, Min(0.1f)] private float _fragmentLifetime = 8f;
    [SerializeField] private bool _useWorldGravity;
    [SerializeField] private bool _randomizeSeed = true;
    [SerializeField] private int _randomSeed = 12345;
    [SerializeField] private Material _insideMaterial;

    [field : SerializeField]
    public float RestoreTime { get; private set; }
    public float FragmentLifetime { get { return _fragmentLifetime; } }

    public bool TryFractureFromStomp(SurfaceInstance surface, Collision collision)
    {
        if (collision == null || collision.contactCount == 0)
        {
            return false;
        }

        return TryFractureFromStomp(surface, collision.GetContact(0).point);
    }

    public bool TryFractureFromStomp(SurfaceInstance surface, Vector3 contactPoint)
    {
        if (surface == null || surface.Profile == null || !surface.Profile.AllowStompFracture)
        {
            return false;
        }

        return BeginFracture(surface, contactPoint);
    }

    public bool TryFractureFromMeteor(SurfaceInstance surface, Vector3 impactPoint)
    {
        return BeginFracture(surface, impactPoint);
    }

    private bool BeginFracture(SurfaceInstance surface, Vector3 burstOrigin)
    {
        if (surface == null || surface.Profile == null)
        {
            return false;
        }

        MeshGlass glass = surface.GetComponent<MeshGlass>();
        if (glass == null)
        {
            Debug.LogError("FractureEffect requires MeshGlass on the SurfaceInstance object.", surface);
            return false;
        }

        if (glass.IsBroken || glass.IsFracturing)
        {
            return false;
        }

        glass.BeginFracture(this, burstOrigin);
        return true;
    }

    // OpenFracture로 실제 메시를 절단하며 한 프레임에 한 번씩 분할한다.
    public IEnumerator Fracture(MeshGlass glass, Vector3 burstOrigin)
    {
        Mesh source = glass.GetComponent<MeshFilter>().sharedMesh;
        if (source == null || !source.isReadable || source.subMeshCount != 1)
        {
            throw new InvalidOperationException("Glass requires a readable, closed mesh with one submesh.");
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
        int seed = _randomizeSeed ? Guid.NewGuid().GetHashCode() : _randomSeed;
        System.Random random = new System.Random(seed);
        Debug.Log("FractureEffect generated random seed: " + seed, glass);
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
            Vector3 outwardDirection = shard.transform.position - burstOrigin;
            if (outwardDirection.sqrMagnitude < 0.0001f)
            {
                // 중심과 정확히 겹치는 조각도 멈춰 있지 않도록 무작위 방사 방향을 준다.
                outwardDirection = new Vector3((float)random.NextDouble() - 0.5f,
                    (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f);
            }

            Vector3 velocity = outwardDirection.normalized * _fragmentSpeed;
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
