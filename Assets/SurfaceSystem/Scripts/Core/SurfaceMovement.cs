using System.Reflection;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class SurfaceMovement : MonoBehaviour
{
    private static readonly FieldInfo PlayerGravityInfo = typeof(PlayerMovement).GetField("_gravityInfo", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo PlayerHorizontalVelocity = typeof(PlayerMovement).GetField("_xVelocity", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo PlayerVerticalVelocity = typeof(PlayerMovement).GetField("_yVelocity", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo PlayerGrounded = typeof(PlayerMovement).GetField("_isGrounded", BindingFlags.Instance | BindingFlags.NonPublic);

    private SurfaceInstance _source;
    private Rigidbody _body;
    private PlayerMovement _player;
    private IGravityInfo _gravityInfo;
    private float _externalVelocityUntil;

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

    public void ApplyExternalVelocity(Vector3 velocity)
    {
        if (_body == null)
            _body = GetComponent<Rigidbody>();
        if (_body == null)
            return;

        _body.linearVelocity = velocity;

        if (_player == null)
            _player = GetComponent<PlayerMovement>();
        if (_player == null)
            return;

        IGravityInfo gravityInfo = PlayerGravityInfo?.GetValue(_player) as IGravityInfo;
        if (_gravityInfo != gravityInfo)
        {
            if (_gravityInfo != null)
                _gravityInfo.GravityOriginChanged -= OnGravityOriginChanged;
            _gravityInfo = gravityInfo;
            if (_gravityInfo != null)
                _gravityInfo.GravityOriginChanged += OnGravityOriginChanged;
        }

        _externalVelocityUntil = Time.time + 1f;
        SyncPlayerVelocity(velocity, _gravityInfo?.GravityOrigin);
    }

    public void SetGroundedForSurface(bool grounded)
    {
        if (_player == null)
            _player = GetComponent<PlayerMovement>();
        if (_player != null)
            PlayerGrounded?.SetValue(_player, grounded);
    }

    private void OnGravityOriginChanged(GravitySource newSource)
    {
        if (Time.time <= _externalVelocityUntil && _body != null)
            SyncPlayerVelocity(_body.linearVelocity, newSource);
    }

    private void SyncPlayerVelocity(Vector3 velocity, GravitySource gravitySource)
    {
        Vector3 outward = gravitySource != null
            ? (transform.position - gravitySource.transform.position).normalized
            : transform.up;
        Vector3 tangent = Vector3.Cross(outward, Vector3.forward).normalized;

        PlayerHorizontalVelocity?.SetValue(_player, Vector3.Dot(velocity, tangent));
        PlayerVerticalVelocity?.SetValue(_player, Vector3.Dot(velocity, outward));
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
        if (_gravityInfo != null)
            _gravityInfo.GravityOriginChanged -= OnGravityOriginChanged;
        _gravityInfo = null;
        _source = null;
        Current = MovementMultipliers.Default;
    }
}
