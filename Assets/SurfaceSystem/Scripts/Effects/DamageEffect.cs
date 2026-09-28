using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "SurfaceSystem/Effects/Damage Effect")]
public class DamageEffect : SurfaceEffect
{
    [SerializeField, Min(0f)] private float _damage = 10f;

    public override void OnSinkThresholdReached(SurfaceInstance surface, Rigidbody body, Vector3 outward)
    {
        if (body == null || _damage <= 0f)
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
}

public interface ISurfaceDamageReceiver
{
    void TakeSurfaceDamage(float amount);
}
