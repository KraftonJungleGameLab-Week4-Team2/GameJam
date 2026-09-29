using UnityEngine;

[CreateAssetMenu(fileName = "CrushEffect", menuName = "SurfaceSystem/Effects/Crush Effect")]
public class CrushEffect : SurfaceEffect
{
    [Header("Impact")]
    [SerializeField, Min(0f)] private float _minimumImpactSpeed = 0.2f;

    [Header("Permanent Dent")]
    [SerializeField, Min(0.01f)] private float _dentRadius = 1.5f;
    [SerializeField, Min(0f)] private float _depthPerSpeed = 0.25f;
    [SerializeField, Min(0.01f)] private float _maximumDentDepth = 1f;

    public override void OnImpact(SurfaceInstance surface, Collision collision)
    {
        if (collision.rigidbody == null || collision.rigidbody.isKinematic || collision.contactCount == 0)
        {
            return;
        }

        ContactPoint contact = collision.GetContact(0);
        Vector3 normal = contact.normal;
        if (Vector3.Dot(normal, collision.rigidbody.worldCenterOfMass - contact.point) < 0f)
        {
            normal = -normal;
        }
        float impactSpeed = -Vector3.Dot(collision.relativeVelocity, normal);
        ApplyDent(surface, contact.point, -normal, impactSpeed);
    }

    public override void OnSurfaceTriggerImpact(SurfaceInstance surface, Collider other)
    {
        Rigidbody body = other != null ? other.attachedRigidbody : null;
        if (surface == null || body == null || body.isKinematic)
        {
            return;
        }

        Vector3 point = surface.GetTriggerContactPoint(other);
        Vector3 normal = surface.GetTriggerSurfaceNormal(other, point);
        Rigidbody surfaceBody = surface.GetComponent<Rigidbody>();
        Vector3 relativeVelocity = body.GetPointVelocity(point);
        if (surfaceBody != null)
        {
            relativeVelocity -= surfaceBody.GetPointVelocity(point);
        }

        float impactSpeed = -Vector3.Dot(relativeVelocity, normal);
        ApplyDent(surface, point, -normal, impactSpeed);
    }

    private void ApplyDent(SurfaceInstance surface, Vector3 point, Vector3 inwardDirection, float impactSpeed)
    {
        if (impactSpeed < _minimumImpactSpeed)
        {
            return;
        }

        MeshAluminium aluminium = surface.GetComponent<MeshAluminium>();
        if (aluminium == null)
        {
            Debug.LogError("CrushEffect requires MeshAluminium on the SurfaceInstance object.", surface);
            return;
        }

        float depth = Mathf.Min(_maximumDentDepth,
            (impactSpeed - _minimumImpactSpeed) * _depthPerSpeed);
        aluminium.ApplyImpact(point, inwardDirection, depth, _dentRadius, _maximumDentDepth);
    }
}
