using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class SurfaceTrigger : MonoBehaviour
{
    private SurfaceInstance _surface;
    private Collider _triggerCollider;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
        _surface = GetComponentInParent<SurfaceInstance>();
        enabled = _triggerCollider != null && _triggerCollider.isTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_surface != null && _triggerCollider != null)
        {
            _surface.HandleTriggerEnter(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_surface != null && _triggerCollider != null)
        {
            _surface.HandleTriggerStay(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_surface != null && _triggerCollider != null)
        {
            _surface.HandleTriggerExit(other);
        }
    }
}
