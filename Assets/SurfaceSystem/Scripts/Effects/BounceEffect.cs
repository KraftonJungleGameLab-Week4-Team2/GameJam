using UnityEngine;

[CreateAssetMenu(fileName = "BounceEffect", menuName = "SurfaceSystem/Effects/Bounce Effect")]
public class BounceEffect : SurfaceEffect
{
    [Header("Impact Bounce")]
    [SerializeField, Min(0f)] private float _bounceMultiplier = 1.2f;
    [SerializeField, Min(0f)] private float _minimumImpactSpeed = 1f;
    [SerializeField, Min(0f)] private float _maxBounceSpeed = 20f;

    [Header("Contact Bounce")]
    [SerializeField, Min(0f)] private float _groundedBounceSpeed = 2f;

    public override void OnImpact(SurfaceInstance surface, Collision collision)
    {
        Rigidbody rigidbody = collision.rigidbody;

        if (rigidbody == null || rigidbody.isKinematic || collision.contactCount == 0)
        {
            return;
        }

        Vector3 surfaceNormal = GetSurfaceNormal(collision);

        float incomingNormalVelocity = Vector3.Dot(collision.relativeVelocity, surfaceNormal);

        if (incomingNormalVelocity >= -_minimumImpactSpeed)
        {
            return;
        }

        float bounceSpeed = -incomingNormalVelocity * _bounceMultiplier;
        bounceSpeed = Mathf.Min(bounceSpeed, _maxBounceSpeed);

        float currentNormalVelocity = Vector3.Dot(rigidbody.linearVelocity, surfaceNormal);
        float velocityChange = bounceSpeed - currentNormalVelocity;

        if (velocityChange <= 0f)
        {
            return;
        }

        rigidbody.AddForce(surfaceNormal * velocityChange, ForceMode.VelocityChange);
    }

    public override void OnStay(SurfaceInstance surface, Collision collision)
    {
        Rigidbody rigidbody = collision.rigidbody;

        if (rigidbody == null || rigidbody.isKinematic || _groundedBounceSpeed <= 0f || collision.contactCount == 0)
        {
            return;
        }

        Vector3 surfaceNormal = GetSurfaceNormal(collision);
        float currentNormalVelocity = Vector3.Dot(rigidbody.linearVelocity, surfaceNormal);

        if (currentNormalVelocity > 0f)
        {
            return;
        }

        rigidbody.AddForce(surfaceNormal * _groundedBounceSpeed, ForceMode.VelocityChange);
    }

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