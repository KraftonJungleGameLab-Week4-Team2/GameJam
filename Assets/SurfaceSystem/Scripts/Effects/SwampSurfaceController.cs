using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10001)]
[DisallowMultipleComponent]
public sealed class SwampSurfaceController : MonoBehaviour
{
    private struct CollisionPair
    {
        public Collider player;
        public Collider planet;
        public bool wasIgnored;
    }

    private readonly List<CapsuleCollider> _capsules = new List<CapsuleCollider>();
    private readonly List<float> _originalHeights = new List<float>();
    private readonly List<float> _originalRadii = new List<float>();
    private readonly List<CollisionPair> _ignoredPairs = new List<CollisionPair>();

    private SinkEffect _effect;
    private Rigidbody _playerBody;
    private Collider _playerCollider;
    private Collider _trigger;
    private float _depth;
    private bool _active;
    private bool _waitUntilExit;

    public void Begin(SinkEffect effect, Collider playerCollider)
    {
        if (effect == null || playerCollider == null || playerCollider.attachedRigidbody == null)
        {
            return;
        }

        if (_waitUntilExit)
        {
            if (Overlaps(_trigger, _playerCollider))
            {
                return;
            }
            _waitUntilExit = false;
            _trigger = null;
            _playerCollider = null;
        }

        if (_active)
        {
            return;
        }

        _effect = effect;
        _playerBody = playerCollider.attachedRigidbody;
        _playerCollider = playerCollider;
        _trigger = FindTrigger(playerCollider);
        _depth = 0f;
        CacheCapsules();
        SetPlanetCollisionIgnored(true);
        _active = true;
    }

    public void End(SurfaceInstance surface)
    {
        if (!_active || surface != GetComponent<SurfaceInstance>())
        {
            return;
        }

        if (Overlaps(_trigger, _playerCollider))
        {
            return;
        }

        Restore(false);
    }

    private void FixedUpdate()
    {
        if (_waitUntilExit)
        {
            if (!Overlaps(_trigger, _playerCollider))
            {
                _waitUntilExit = false;
                _trigger = null;
                _playerCollider = null;
            }
            return;
        }

        if (!_active || _effect == null || _playerBody == null)
        {
            return;
        }

        if (_trigger != null && !Overlaps(_trigger, _playerCollider))
        {
            Restore(false);
            return;
        }

        Vector3 outward = (_playerBody.worldCenterOfMass - transform.position).normalized;
        if (outward.sqrMagnitude < 0.5f)
        {
            outward = transform.up;
        }

        Vector3 velocity = _playerBody.linearVelocity;
        float radialSpeed = Vector3.Dot(velocity, outward);
        if (_depth > 0.02f && radialSpeed > 1f)
        {
            Restore(true);
            return;
        }

        _depth = Mathf.Min(_effect.MaximumSinkDepth, _depth + _effect.SinkSpeed * Time.fixedDeltaTime);
        float progress = _effect.MaximumSinkDepth > 0f ? _depth / _effect.MaximumSinkDepth : 1f;
        ApplyCapsuleScale(Mathf.Lerp(1f, _effect.MinimumColliderScale, progress));

        Vector3 tangent = Vector3.ProjectOnPlane(velocity, outward);
        radialSpeed = Mathf.Max(radialSpeed, -_effect.SinkSpeed);
        _playerBody.linearVelocity = tangent + outward * radialSpeed;

        if (_depth >= Mathf.Min(_effect.ThresholdDepth, _effect.MaximumSinkDepth))
        {
            SurfaceInstance surface = GetComponent<SurfaceInstance>();
            if (surface != null && surface.Profile != null)
            {
                surface.Profile.ProcessSinkThresholdReached(surface, _playerBody, outward);
            }
            Restore(true);
        }
    }

    private Collider FindTrigger(Collider playerCollider)
    {
        foreach (Collider candidate in GetComponentsInChildren<Collider>(true))
        {
            if (candidate.isTrigger && candidate.bounds.Intersects(playerCollider.bounds))
            {
                return candidate;
            }
        }
        return null;
    }

    private bool Overlaps(Collider trigger, Collider player)
    {
        return trigger != null && player != null && trigger.bounds.Intersects(player.bounds);
    }

    private void CacheCapsules()
    {
        _capsules.Clear();
        _originalHeights.Clear();
        _originalRadii.Clear();
        foreach (CapsuleCollider capsule in _playerBody.GetComponentsInChildren<CapsuleCollider>(true))
        {
            if (capsule.attachedRigidbody != _playerBody)
            {
                continue;
            }
            _capsules.Add(capsule);
            _originalHeights.Add(capsule.height);
            _originalRadii.Add(capsule.radius);
        }
    }

    private void ApplyCapsuleScale(float scale)
    {
        for (int i = 0; i < _capsules.Count; i++)
        {
            CapsuleCollider capsule = _capsules[i];
            if (capsule == null)
            {
                continue;
            }
            capsule.radius = _originalRadii[i] * scale;
            capsule.height = Mathf.Max(capsule.radius * 2f, _originalHeights[i] * scale);
        }
    }

    private void SetPlanetCollisionIgnored(bool ignored)
    {
        if (ignored)
        {
            _ignoredPairs.Clear();
            Collider[] playerColliders = _playerBody.GetComponentsInChildren<Collider>(true);
            Collider[] planetColliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider player in playerColliders)
            {
                if (player == null || player.isTrigger)
                {
                    continue;
                }
                foreach (Collider planet in planetColliders)
                {
                    if (planet == null || planet.isTrigger)
                    {
                        continue;
                    }
                    bool wasIgnored = Physics.GetIgnoreCollision(player, planet);
                    _ignoredPairs.Add(new CollisionPair { player = player, planet = planet, wasIgnored = wasIgnored });
                    Physics.IgnoreCollision(player, planet, true);
                }
            }
            return;
        }

        foreach (CollisionPair pair in _ignoredPairs)
        {
            if (pair.player != null && pair.planet != null)
            {
                Physics.IgnoreCollision(pair.player, pair.planet, pair.wasIgnored);
            }
        }
        _ignoredPairs.Clear();
    }

    private void Restore(bool waitUntilExit)
    {
        ApplyCapsuleScale(1f);
        SetPlanetCollisionIgnored(false);
        _active = false;
        _depth = 0f;
        _effect = null;
        _playerBody = null;
        _waitUntilExit = waitUntilExit;
        if (!waitUntilExit)
        {
            _playerCollider = null;
            _trigger = null;
        }
    }

    private void OnDisable()
    {
        Restore(false);
    }
}
