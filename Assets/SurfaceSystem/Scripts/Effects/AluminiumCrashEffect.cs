using UnityEngine;

[CreateAssetMenu(fileName = "AluminiumCrashEffect", menuName = "SurfaceSystem/Effects/Aluminium Crash Effect")]
public class AluminiumCrashEffect : SurfaceEffect
{
    [Header("Impact")]
    [SerializeField, Min(0f)] private float _minimumImpactSpeed = 0.2f;

    [Header("Permanent Dent")]
    [SerializeField, Min(0.01f)] private float _dentRadius = 1.5f;
    [SerializeField, Min(0f)] private float _depthPerSpeed = 0.25f;
    [SerializeField, Min(0.01f)] private float _maximumDentDepth = 1f;

    public override void OnImpact(SurfaceInstance surface, Collision collision)
    {
        if (collision.contactCount == 0 || collision.rigidbody == null || collision.rigidbody.isKinematic)
        {
            return;
        }

        ContactPoint contact = collision.GetContact(0);
        Vector3 outward = contact.normal;
        Vector3 directionToBody = collision.rigidbody.worldCenterOfMass - contact.point;
        if (Vector3.Dot(outward, directionToBody) < 0f)
        {
            outward = -outward;
        }

        float impactSpeed = -Vector3.Dot(collision.relativeVelocity, outward.normalized);
        if (impactSpeed < _minimumImpactSpeed)
        {
            return;
        }

        MeshAluminium aluminium = surface.GetComponent<MeshAluminium>();
        if (aluminium == null)
        {
            Debug.LogError("AluminiumCrashEffect requires MeshAluminium on the SurfaceInstance object.", surface);
            return;
        }

        float depth = Mathf.Min(_maximumDentDepth,
            (impactSpeed - _minimumImpactSpeed) * _depthPerSpeed);
        aluminium.ApplyImpact(contact.point, -outward.normalized, depth, _dentRadius, _maximumDentDepth);
    }
}
