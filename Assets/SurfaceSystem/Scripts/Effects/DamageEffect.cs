using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "SurfaceSystem/Effects/Damage Effect")]
public class DamageEffect : SurfaceEffect
{
    [SerializeField, Min(0f)] private float _damage = 10f;
    [SerializeField] private bool _onlyAffectPlayer;

    public override void OnEnter(SurfaceInstance surface, Collision collision)
    {
        ApplyDamage(collision != null ? collision.collider : null);
    }

    public override void OnSurfaceTriggerEnter(SurfaceInstance surface, Collider other)
    {
        ApplyDamage(other);
    }

    public override void OnSinkThresholdReached(SurfaceInstance surface, Rigidbody body, Vector3 outward)
    {
        ApplyDamage(body);
    }

    private void ApplyDamage(Collider target)
    {
        if (target == null)
        {
            return;
        }

        Rigidbody body = target.attachedRigidbody;
        if (body == null)
        {
            return;
        }

        ApplyDamage(body);
    }

    private void ApplyDamage(Rigidbody body)
    {
        if (body == null || _damage <= 0f || (_onlyAffectPlayer && !HasPlayerMovement(body)))
        {
            return;
        }

        foreach (MonoBehaviour behaviour in body.GetComponentsInParent<MonoBehaviour>())
        {
            if (behaviour is ISurfaceDamageReceiver receiver)
            {
                receiver.TakeSurfaceDamage(_damage);
                return;
            }
        }

        // No player assembly dependency: existing health scripts can expose TakeDamage(float).
        body.gameObject.SendMessageUpwards("TakeDamage", _damage, SendMessageOptions.DontRequireReceiver);
    }

    private bool HasPlayerMovement(Rigidbody body)
    {
        foreach (MonoBehaviour behaviour in body.GetComponentsInParent<MonoBehaviour>())
        {
            if (behaviour != null && behaviour.GetType().Name == "PlayerMovement")
            {
                return true;
            }
        }
        return false;
    }
}

public interface ISurfaceDamageReceiver
{
    void TakeSurfaceDamage(float amount);
}
