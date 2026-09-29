using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(SphereCollider), typeof(SurfaceInstance))]
public class MeshAluminium : MonoBehaviour, IMeshDeformationTarget
{
    [Header("Direct Deformation Input")]
    [SerializeField, Min(0.01f)] private float _dentRadius = 1.5f;
    [SerializeField, Min(0f)] private float _depthPerSpeed = 0.08f;
    [SerializeField, Min(0.01f)] private float _maximumDentDepth = 1f;

    private struct DentRequest
    {
        public Vector3 point;
        public Vector3 inwardDirection;
        public float depth;
        public float radius;
        public float maximumDepth;
    }

    private readonly Queue<DentRequest> _pendingDents = new Queue<DentRequest>();
    private Mesh _mesh;
    private Vector3[] _originalVertices;
    private Vector3[] _deformedVertices;
    private bool _ready;

    private void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        Mesh source = meshFilter.sharedMesh;
        if (source == null || !source.isReadable)
        {
            Debug.LogError("MeshAluminium requires a readable MeshFilter mesh.", this);
            enabled = false;
            return;
        }

        _mesh = Instantiate(source);
        _mesh.name = source.name + " (Aluminium Deformation)";
        _mesh.MarkDynamic();
        meshFilter.sharedMesh = _mesh;

        _originalVertices = _mesh.vertices;
        _deformedVertices = (Vector3[])_originalVertices.Clone();

        _ready = true;
    }

    // AluminiumCrashEffect가 충격을 판정한 뒤 시각 메시 변형만 요청한다.
    public void ApplyImpact(Vector3 worldPoint, Vector3 inwardDirection, float depth,
        float dentRadius, float maximumDentDepth)
    {
        if (!_ready || depth <= 0f || inwardDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        QueueDent(worldPoint, inwardDirection.normalized, depth, dentRadius, maximumDentDepth);
    }

    // 마우스 입력 등 기존 변형 도구에서도 영구 눌림을 줄 수 있다.
    public void ApplyDeformationForce(Vector3 worldPoint, Vector3 worldForce, float deltaTime)
    {
        if (!_ready || worldForce.sqrMagnitude <= 0f)
        {
            return;
        }

        float dentDepth = worldForce.magnitude * deltaTime * _depthPerSpeed;
        QueueDent(worldPoint, worldForce.normalized, dentDepth, _dentRadius, _maximumDentDepth);
    }

    private void QueueDent(Vector3 point, Vector3 inwardDirection, float depth,
        float radius, float maximumDepth)
    {
        if (depth > 0f)
        {
            _pendingDents.Enqueue(new DentRequest
            {
                point = point,
                inwardDirection = inwardDirection,
                depth = depth,
                radius = Mathf.Max(0.01f, radius),
                maximumDepth = Mathf.Max(0.01f, maximumDepth)
            });
        }
    }

    // 충돌 콜백에서는 대기열만 채우고 다음 물리 틱에 렌더 메시를 갱신한다.
    private void FixedUpdate()
    {
        if (!_ready || _pendingDents.Count == 0)
        {
            return;
        }

        while (_pendingDents.Count > 0)
        {
            ApplyDent(_pendingDents.Dequeue());
        }

        _mesh.vertices = _deformedVertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        // 충돌 형상은 SphereCollider가 계속 담당하고, 여기서는 렌더 메시만 영구 변형한다.
    }

    private void ApplyDent(DentRequest dent)
    {
        float radius = dent.radius;
        float maxDepth = dent.maximumDepth;
        float maxDepthSqr = maxDepth * maxDepth;

        for (int i = 0; i < _deformedVertices.Length; i++)
        {
            Vector3 currentWorld = transform.TransformPoint(_deformedVertices[i]);
            float distance = Vector3.Distance(currentWorld, dent.point);
            if (distance >= radius)
            {
                continue;
            }

            float falloff = 1f - distance / radius;
            falloff = falloff * falloff * (3f - 2f * falloff);

            Vector3 originalWorld = transform.TransformPoint(_originalVertices[i]);
            Vector3 totalOffset = currentWorld - originalWorld;
            totalOffset += dent.inwardDirection * (dent.depth * falloff);
            if (totalOffset.sqrMagnitude > maxDepthSqr)
            {
                totalOffset = totalOffset.normalized * maxDepth;
            }

            _deformedVertices[i] = transform.InverseTransformPoint(originalWorld + totalOffset);
        }
    }

    private void OnDestroy()
    {
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
    }
}
