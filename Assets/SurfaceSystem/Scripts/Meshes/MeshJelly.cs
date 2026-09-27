using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshJelly : MonoBehaviour, IMeshDeformationTarget
{
    [Header("Deformation")]
    [SerializeField, Min(0.01f)] private float _deformationRadius = 0.2f;
    [SerializeField, Min(0f)] private float _forceMultiplier = 1f;
    [SerializeField, Min(0f)] private float _springForce = 20f;
    [SerializeField, Min(0f)] private float _damping = 5f;
    [SerializeField, Min(0.0001f)] private float _settleThreshold = 0.001f;

    [Header("Collision Deformation")]
    [SerializeField, Min(0f)] private float _minimumImpactSpeed = 1f;
    [SerializeField, Min(0f)] private float _impactForceMultiplier = 5f;

    private Mesh _mesh;
    private Vector3[] _originalVertices;
    private Vector3[] _deformedVertices;
    private Vector3[] _vertexVelocities;
    private bool _isDeforming;

    void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        _mesh = Instantiate(meshFilter.sharedMesh);
        _mesh.MarkDynamic();
        meshFilter.sharedMesh = _mesh;

        _originalVertices = _mesh.vertices;
        _deformedVertices = _mesh.vertices;
        _vertexVelocities = new Vector3[_mesh.vertexCount];
    }

    void LateUpdate()
    {
        if (_isDeforming)
        {
            UpdateDeformation();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount == 0 || collision.rigidbody == null)
        {
            return;
        }

        if (collision.gameObject.GetComponentInParent<SurfaceAgent>() == null)
        {
            return;
        }

        ContactPoint contact = collision.GetContact(0);
        Vector3 surfaceNormal = GetSurfaceNormal(collision, contact);
        float impactSpeed = -Vector3.Dot(collision.relativeVelocity, surfaceNormal);

        if (impactSpeed < _minimumImpactSpeed)
        {
            return;
        }

        ApplyDeformationForce(
            contact.point,
            -surfaceNormal * impactSpeed * _impactForceMultiplier,
            Time.fixedDeltaTime);
    }

    void OnDestroy()
    {
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
    }

    // 입력이나 충돌로 받은 월드 좌표와 힘을 주변 버텍스의 속도에 더한다.
    public void ApplyDeformationForce(Vector3 worldPoint, Vector3 worldForce, float deltaTime)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        Vector3 localForce = transform.InverseTransformVector(worldForce) * _forceMultiplier;

        for (int index = 0; index < _deformedVertices.Length; index++)
        {
            float distance = Vector3.Distance(_deformedVertices[index], localPoint);
            if (distance > _deformationRadius)
            {
                continue;
            }

            float falloff = 1f - distance / _deformationRadius;
            _vertexVelocities[index] += localForce * falloff * deltaTime;
        }

        _isDeforming = true;
    }

    // 모든 버텍스를 원래 위치로 끌어당기고 감쇠시켜 젤리처럼 복원한다.
    private void UpdateDeformation()
    {
        float deltaTime = Time.deltaTime;
        float damping = 1f / (1f + _damping * deltaTime);
        float thresholdSqr = _settleThreshold * _settleThreshold;
        bool isMoving = false;

        for (int index = 0; index < _deformedVertices.Length; index++)
        {
            Vector3 returnForce = _originalVertices[index] - _deformedVertices[index];
            _vertexVelocities[index] += returnForce * _springForce * deltaTime;
            _vertexVelocities[index] *= damping;
            _deformedVertices[index] += _vertexVelocities[index] * deltaTime;

            if (_vertexVelocities[index].sqrMagnitude > thresholdSqr
                || returnForce.sqrMagnitude > thresholdSqr)
            {
                isMoving = true;
            }
        }

        _mesh.vertices = _deformedVertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _isDeforming = isMoving;
    }

    // 접촉면의 수직 방향이 행성 표면에서 충돌 물체를 향하도록 맞춘다.
    private Vector3 GetSurfaceNormal(Collision collision, ContactPoint contact)
    {
        Vector3 surfaceNormal = contact.normal;
        Vector3 directionToBody = collision.rigidbody.worldCenterOfMass - contact.point;

        if (Vector3.Dot(surfaceNormal, directionToBody) < 0f)
        {
            surfaceNormal = -surfaceNormal;
        }

        return surfaceNormal.normalized;
    }
}
