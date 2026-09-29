using UnityEngine;

[DefaultExecutionOrder(10000)]
public class SurfaceMovement : MonoBehaviour
{
    private SurfaceInstance _source;
    private Rigidbody _body;

    public MovementMultipliers Current { get; private set; } = MovementMultipliers.Default;

    public void Apply(SurfaceInstance source, MovementMultipliers modifiers)
    {
        _source = source;
        Current = modifiers;
    }

    public void Clear(SurfaceInstance source)
    {
        if (_source != source)
        {
            return;
        }

        _source = null;
        Current = MovementMultipliers.Default;
    }

    private void FixedUpdate()
    {
        float speedMultiplier = Mathf.Clamp(Current.maxSpeedMultiplier, 0f, 10f);
        if (Mathf.Abs(speedMultiplier - 1f) < 0.001f)
            return;
        if (_body == null)
            _body = GetComponent<Rigidbody>();
        if (_body == null)
            return;

        Vector3 outward = transform.up;
        Vector3 velocity = _body.linearVelocity;
        Vector3 tangent = Vector3.ProjectOnPlane(velocity, outward);
        float radialSpeed = Vector3.Dot(velocity, outward);
        _body.linearVelocity = tangent * speedMultiplier + outward * radialSpeed;
    }

    private void OnDisable()
    {
        _source = null;
        Current = MovementMultipliers.Default;
    }
}
