using UnityEngine;

[CreateAssetMenu(fileName = "BounceEffect", menuName = "SurfaceSystem/Effects/Bounce Effect")]
public class BounceEffect : SurfaceEffect
{
    [Header("Impact Bounce")]
    [SerializeField, Min(0f)] private float _bounceMultiplier = 1.2f;
    [SerializeField, Min(0f)] private float _minimumImpactSpeed = 1f;
    [SerializeField, Min(0f)] private float _maxBounceSpeed = 20f;
    [SerializeField] private bool _onlyAffectPlayer;

    public void ApplyOutwardBounce(Rigidbody body, Vector3 outward)
    {
        if (body == null || body.isKinematic)
        {
            return;
        }

        outward.Normalize();
        float bounceSpeed = _maxBounceSpeed;
        ApplyVelocity(body, Vector3.ProjectOnPlane(body.linearVelocity, outward) + outward * bounceSpeed);
    }

    public override void OnSinkThresholdReached(SurfaceInstance surface, Rigidbody body, Vector3 outward)
    {
        ApplyOutwardBounce(body, outward);
    }

    public override void OnImpact(SurfaceInstance surface, Collision collision)
    {
        Rigidbody rigidbody = collision.rigidbody;

        if (rigidbody == null || rigidbody.isKinematic || collision.contactCount == 0 || !IsValidTarget(rigidbody))
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

        ApplyVelocity(rigidbody, rigidbody.linearVelocity + surfaceNormal * velocityChange);
    }

    public override void OnSurfaceTriggerImpact(SurfaceInstance surface, Collider other)
    {
        Rigidbody rigidbody = other != null ? other.attachedRigidbody : null;
        if (surface == null || rigidbody == null || rigidbody.isKinematic || !IsValidTarget(rigidbody))
        {
            return;
        }

        Vector3 contactPoint = surface.GetTriggerContactPoint(other);
        Vector3 surfaceNormal = surface.GetTriggerSurfaceNormal(other, contactPoint);
        Rigidbody surfaceBody = surface.GetComponent<Rigidbody>();
        Vector3 relativeVelocity = rigidbody.GetPointVelocity(contactPoint);
        if (surfaceBody != null)
        {
            relativeVelocity -= surfaceBody.GetPointVelocity(contactPoint);
        }

        float incomingNormalVelocity = Vector3.Dot(relativeVelocity, surfaceNormal);
        if (incomingNormalVelocity >= -_minimumImpactSpeed)
        {
            return;
        }

        float bounceSpeed = Mathf.Min(-incomingNormalVelocity * _bounceMultiplier, _maxBounceSpeed);
        float currentNormalVelocity = Vector3.Dot(rigidbody.linearVelocity, surfaceNormal);
        float velocityChange = bounceSpeed - currentNormalVelocity;
        if (velocityChange > 0f)
        {
            ApplyVelocity(rigidbody, rigidbody.linearVelocity + surfaceNormal * velocityChange);
        }
    }

    private void ApplyVelocity(Rigidbody body, Vector3 velocity)
    {
        SurfaceMovement movement = body.GetComponent<SurfaceMovement>();
        if (movement != null)
            movement.ApplyExternalVelocity(velocity);
        else
            body.linearVelocity = velocity;
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

    private bool IsValidTarget(Rigidbody body)
    {
        if (!_onlyAffectPlayer)
        {
            return true;
        }

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
