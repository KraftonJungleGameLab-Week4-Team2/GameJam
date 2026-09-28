using UnityEngine;

[CreateAssetMenu(fileName = "SwampEffect", menuName = "SurfaceSystem/Effects/Swamp Effect")]
public class SwampEffect : SurfaceEffect
{
    [Header("Sinking")]
    [SerializeField, Min(0.01f)] private float _sinkSpeed = 0.45f;
    [SerializeField, Min(0.01f)] private float _maximumSinkDepth = 1.2f;
    [SerializeField, Range(0.05f, 1f)] private float _minimumColliderScale = 0.35f;
    [Header("Damage and Ejection")]
    [SerializeField, Min(0f)] private float _damageDepth = 0.9f;
    [SerializeField, Min(0f)] private float _damage = 10f;

    internal float SinkSpeed { get { return _sinkSpeed; } }
    internal float MaximumSinkDepth { get { return _maximumSinkDepth; } }
    internal float MinimumColliderScale { get { return _minimumColliderScale; } }
    internal float DamageDepth { get { return _damageDepth; } }
    internal float Damage { get { return _damage; } }

    public override void OnEnter(SurfaceInstance surface, Collision collision) => Begin(surface, collision?.collider);
    public override void OnSurfaceTriggerEnter(SurfaceInstance surface, Collider other) => Begin(surface, other);
    public override void OnStay(SurfaceInstance surface, Collision collision) => Begin(surface, collision?.collider);
    public override void OnSurfaceTriggerStay(SurfaceInstance surface, Collider other) => Begin(surface, other);
    public override void OnExit(SurfaceInstance surface, Collision collision) => End(surface, collision?.collider);
    public override void OnSurfaceTriggerExit(SurfaceInstance surface, Collider other) => End(surface, other);

    private void Begin(SurfaceInstance surface, Collider other)
    {
        if (surface == null || other == null)
        {
            return;
        }

        SwampSurfaceController controller = surface.GetComponent<SwampSurfaceController>();
        if (controller == null)
        {
            return;
        }
        controller.Begin(this, other);
    }

    private void End(SurfaceInstance surface, Collider other)
    {
        if (surface == null || other == null)
        {
            return;
        }

        SwampSurfaceController controller = surface.GetComponent<SwampSurfaceController>();
        if (controller != null)
        {
            controller.End(surface);
        }
    }

    internal void OnDepthReached(SurfaceInstance surface, Rigidbody body, Vector3 outward)
    {
        if (surface == null || body == null)
        {
            return;
        }

        if (_damage > 0f)
        {
            bool delivered = false;
            foreach (MonoBehaviour behaviour in body.GetComponentsInParent<MonoBehaviour>())
            {
                if (behaviour is ISurfaceDamageReceiver receiver)
                {
                    receiver.TakeSurfaceDamage(_damage);
                    delivered = true;
                    break;
                }
            }

            if (!delivered)
            {
                body.gameObject.SendMessageUpwards("TakeDamage", _damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        BounceEffect bounce = surface.Profile != null ? surface.Profile.GetEffect<BounceEffect>() : null;
        if (bounce != null)
        {
            bounce.ApplyOutwardBounce(body, outward);
        }
        else
        {
            Debug.LogError("SwampEffect requires a BounceEffect in the same SurfaceProfile.", surface);
        }
    }
}

public interface ISurfaceDamageReceiver
{
    void TakeSurfaceDamage(float amount);
}
