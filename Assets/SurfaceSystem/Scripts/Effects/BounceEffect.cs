using UnityEngine;

[CreateAssetMenu(fileName = "BounceEffect", menuName = "SurfaceSystem/Effects/Bounce Effect")]
public class BounceEffect : SurfaceEffect
{
    [Header("Impact Bounce")]
    [SerializeField, Min(0f)]
    private float _bounceMultiplier = 1.2f;
    [SerializeField, Min(0f)]
    private float _minimumImpactSpeed = 1f;
    [SerializeField, Min(0f)]
    private float _maxBounceSpeed = 20f;

    [Header("Contact Bounce")]
    [SerializeField, Min(0f)]
    private float _groundedBounceSpeed = 2f;

    // 반동 설정을 공통 결과에 채운다. 같은 설정을 여러 번 넣으면 마지막 설정을 사용한다.
    public override void Modify(ref SurfaceModifiers modifiers, float normalizedSpeed)
    {
        modifiers.bounceEnabled = true;
        modifiers.bounceMultiplier = _bounceMultiplier;
        modifiers.minimumImpactSpeed = _minimumImpactSpeed;
        modifiers.maxBounceSpeed = _maxBounceSpeed;
        modifiers.groundedBounceSpeed = _groundedBounceSpeed;
    }

    // 접근 속도의 표면 법선 성분만 사용해 착지 반동을 만든다.
    public override void OnImpact(Collision collision, SurfaceModifiers modifiers)
    {
        Rigidbody rigidbody = collision.rigidbody;

        if (!modifiers.bounceEnabled || !CanBounce(collision))
        {
            return;
        }

        Vector3 surfaceNormal = GetSurfaceNormal(collision);

        float incomingNormalVelocity = Vector3.Dot(collision.relativeVelocity, surfaceNormal);

        if (incomingNormalVelocity >= -modifiers.minimumImpactSpeed)
        {
            return;
        }

        float bounceSpeed = -incomingNormalVelocity * modifiers.bounceMultiplier;
        bounceSpeed = Mathf.Min(bounceSpeed, modifiers.maxBounceSpeed);

        float currentNormalVelocity = Vector3.Dot(rigidbody.linearVelocity, surfaceNormal);
        float velocityChange = bounceSpeed - currentNormalVelocity;

        if (velocityChange <= 0f)
        {
            return;
        }

        rigidbody.AddForce(surfaceNormal * velocityChange, ForceMode.VelocityChange);
    }

    // 표면에 멈추거나 내려가는 물체에 작은 재도약을 적용한다.
    public override void OnStay(Collision collision, SurfaceModifiers modifiers)
    {
        Rigidbody rigidbody = collision.rigidbody;

        if (!modifiers.bounceEnabled || modifiers.groundedBounceSpeed <= 0f || !CanBounce(collision))
        {
            return;
        }

        Vector3 surfaceNormal = GetSurfaceNormal(collision);
        float currentNormalVelocity = Vector3.Dot(rigidbody.linearVelocity, surfaceNormal);

        if (currentNormalVelocity > 0f)
        {
            return;
        }

        rigidbody.AddForce(surfaceNormal * modifiers.groundedBounceSpeed, ForceMode.VelocityChange);
    }

    // 반동은 접점과 Rigidbody가 있는 SurfaceAgent 대상에만 적용한다.
    private bool CanBounce(Collision collision)
    {
        return collision.rigidbody != null
            && collision.contactCount > 0
            && collision.gameObject.GetComponentInParent<SurfaceAgent>() != null;
    }

    // 접촉면의 수직 방향이 표면에서 충돌한 물체를 향하도록 맞춘다.
    private Vector3 GetSurfaceNormal(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 normal = contact.normal;
        Vector3 directionToBody = collision.rigidbody.worldCenterOfMass - contact.point;
        if (Vector3.Dot(normal, directionToBody) < 0f)
        {
            normal = -normal;
        }

        return normal.normalized;
    }
}
