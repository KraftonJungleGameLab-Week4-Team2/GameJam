using UnityEngine;

public class MeshDeformationInput : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera _inputCamera;
    [SerializeField, Min(0f)] private float _maxDistance = 100f;
    [SerializeField] private LayerMask _layerMask = Physics.DefaultRaycastLayers;

    [Header("Deformation")]
    [SerializeField, Min(0f)] private float _force = 10f;

    void Awake()
    {
        if (_inputCamera == null)
        {
            _inputCamera = Camera.main;
        }

        if (_inputCamera == null)
        {
            Debug.LogError("MeshDeformationInput requires an input camera or a MainCamera.", this);
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            HandleInput();
        }
    }

    // 월드 접촉 위치와 표면 안쪽 힘을 변형 대상에 전달한다.
    private void HandleInput()
    {
        Ray inputRay = _inputCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(inputRay, out RaycastHit hit, _maxDistance, _layerMask, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        IMeshDeformationTarget target = hit.collider.GetComponentInParent<IMeshDeformationTarget>();
        if (target == null)
        {
            return;
        }

        target.ApplyDeformationForce(hit.point, -hit.normal * _force, Time.deltaTime);
    }

}