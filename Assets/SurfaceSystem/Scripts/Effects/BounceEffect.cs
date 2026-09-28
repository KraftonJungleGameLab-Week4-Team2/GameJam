using UnityEngine;
using System.Reflection;

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
        body.linearVelocity = Vector3.ProjectOnPlane(body.linearVelocity, outward) + outward * bounceSpeed;

        // Keep the project's custom-gravity controller in sync with the applied bounce.
        foreach (MonoBehaviour component in body.GetComponents<MonoBehaviour>())
        {
            if (component.GetType().Name != "PlayerMovement")
            {
                continue;
            }

            FieldInfo verticalVelocity = component.GetType().GetField(
                "_yVelocity", BindingFlags.Instance | BindingFlags.NonPublic);
            if (verticalVelocity != null && verticalVelocity.FieldType == typeof(float))
            {
                verticalVelocity.SetValue(component, bounceSpeed);
            }
            break;
        }
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

        rigidbody.AddForce(surfaceNormal * velocityChange, ForceMode.VelocityChange);
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
            rigidbody.AddForce(surfaceNormal * velocityChange, ForceMode.VelocityChange);
        }
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
