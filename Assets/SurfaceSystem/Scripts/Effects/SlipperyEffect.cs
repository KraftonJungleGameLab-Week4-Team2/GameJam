using UnityEngine;

[CreateAssetMenu(fileName = "SlipperyEffect", menuName = "SurfaceSystem/Effects/Slippery Effect")]
public class SlipperyEffect : SurfaceEffect
{
    [Header("Slippery Movement")]
    [SerializeField, Range(0f, 1f)] private float _accelerationMultiplier = 0.4f;
    [SerializeField, Range(0f, 1f)] private float _decelerationMultiplier = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _turningMultiplier = 0.25f;
    [SerializeField, Min(0f)] private float _maxSpeedMultiplier = 1f;
    [SerializeField, Min(0f)] private float _jumpMultiplier = 1f;

    public override void OnEnter(SurfaceInstance surface, Collision collision)
    {
        Apply(surface, collision != null ? collision.collider : null);
    }

    public override void OnTriggerEnter(SurfaceInstance surface, Collider other)
    {
        Apply(surface, other);
    }

    private void Apply(SurfaceInstance surface, Collider other)
    {
        SurfaceMovement movementState = other != null ? other.GetComponentInParent<SurfaceMovement>() : null;

        if (movementState == null)
        {
            return;
        }

        MovementMultipliers modifiers = new MovementMultipliers
        {
            accelerationMultiplier = _accelerationMultiplier,
            decelerationMultiplier = _decelerationMultiplier,
            turningMultiplier = _turningMultiplier,
            maxSpeedMultiplier = _maxSpeedMultiplier,
            jumpMultiplier = _jumpMultiplier
        };

        movementState.Apply(surface, modifiers);
    }

    public override void OnExit(SurfaceInstance surface, Collision collision)
    {
        Clear(surface, collision != null ? collision.collider : null);
    }

    public override void OnTriggerExit(SurfaceInstance surface, Collider other)
    {
        Clear(surface, other);
    }

    private void Clear(SurfaceInstance surface, Collider other)
    {
        SurfaceMovement movementState = other != null ? other.GetComponentInParent<SurfaceMovement>() : null;

        if (movementState == null)
        {
            return;
        }

        movementState.Clear(surface);
    }
}
