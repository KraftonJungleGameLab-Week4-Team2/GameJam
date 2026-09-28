using UnityEngine;

public class SurfaceInstance : MonoBehaviour
{
    [Header("Surface")]
    [SerializeField] private SurfaceProfile _profile;
    [SerializeField] private Collider[] _surfaceColliders = new Collider[0];

    [Header("Trigger Fallback")]
    [SerializeField, Min(0f)] private float _triggerFallbackDelay = 0.08f;

    private Collider _triggerContact;
    private Collider _collisionContact;
    private bool _collisionObservedForTrigger;
    private bool _collisionEnterHandled;
    private bool _triggerFallbackHandled;
    private bool _triggerExitSeen;
    private float _triggerFallbackTime;

    public SurfaceProfile Profile { get { return _profile; } }

    private void Awake()
    {
        if (_surfaceColliders == null || _surfaceColliders.Length == 0)
        {
            _surfaceColliders = GetComponentsInChildren<Collider>(true);
        }

        foreach (Collider surfaceCollider in _surfaceColliders)
        {
            if (surfaceCollider != null && _profile != null)
            {
                surfaceCollider.sharedMaterial = _profile.PhysicsMaterial;
            }
        }

    }

    private void Reset()
    {
        _surfaceColliders = GetComponentsInChildren<Collider>(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_profile == null || collision == null || collision.collider == null)
        {
            return;
        }

        Collider other = collision.collider;
        _collisionContact = other;
        _collisionEnterHandled = true;
        if (other == _triggerContact)
        {
            _collisionObservedForTrigger = true;
        }

        if (other != _triggerContact || !_triggerFallbackHandled)
        {
            _profile.ProcessEnter(this, collision);
            _profile.ProcessImpact(this, collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (_profile == null || collision == null || collision.collider == null)
        {
            return;
        }

        Collider other = collision.collider;
        if (_collisionContact != other)
        {
            _collisionContact = other;
            _collisionEnterHandled = false;
        }

        if (other == _triggerContact)
        {
            _collisionObservedForTrigger = true;
        }

        if (!_collisionEnterHandled && (other != _triggerContact || !_triggerFallbackHandled))
        {
            _profile.ProcessEnter(this, collision);
            _profile.ProcessImpact(this, collision);
            _collisionEnterHandled = true;
        }

        _profile.ProcessStay(this, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_profile == null || collision == null || collision.collider == null)
        {
            return;
        }

        Collider other = collision.collider;
        if (other != _triggerContact || !_triggerFallbackHandled)
        {
            _profile.ProcessExit(this, collision);
        }

        if (_collisionContact == other)
        {
            _collisionContact = null;
            _collisionEnterHandled = false;
        }

        if (_triggerContact == other && _triggerExitSeen)
        {
            ClearTriggerContact();
        }
        else if (_triggerContact != other)
        {
            _triggerFallbackHandled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleTriggerExit(other);
    }

    internal void HandleTriggerEnter(Collider other)
    {
        if (_profile == null || other == null)
        {
            return;
        }

        if (_triggerContact == other)
        {
            _triggerExitSeen = false;
            return;
        }

        _triggerContact = other;
        _collisionObservedForTrigger = _collisionContact == other;
        _triggerFallbackHandled = false;
        _triggerExitSeen = false;
        _triggerFallbackTime = Time.fixedTime + _triggerFallbackDelay;
    }

    internal void HandleTriggerStay(Collider other)
    {
        if (_triggerContact != other)
        {
            HandleTriggerEnter(other);
        }
        else if (_profile != null && _triggerFallbackHandled && !_collisionObservedForTrigger)
        {
            _profile.ProcessTriggerStay(this, other);
        }
    }

    internal void HandleTriggerExit(Collider other)
    {
        if (_profile == null || other == null || _triggerContact != other)
        {
            return;
        }

        if (_triggerFallbackHandled)
        {
            _profile.ProcessTriggerExit(this, other);
        }

        if (_collisionContact == other && _triggerFallbackHandled)
        {
            _triggerExitSeen = true;
            return;
        }

        ClearTriggerContact();
    }

    private void FixedUpdate()
    {
        if (_profile == null || _triggerContact == null || _collisionObservedForTrigger || _triggerFallbackHandled
            || Time.fixedTime < _triggerFallbackTime)
        {
            return;
        }

        _triggerFallbackHandled = true;
        _profile.ProcessTriggerEnter(this, _triggerContact);
        _profile.ProcessTriggerImpact(this, _triggerContact);
    }

    private void ClearTriggerContact()
    {
        _triggerContact = null;
        _collisionObservedForTrigger = false;
        _triggerFallbackHandled = false;
        _triggerExitSeen = false;
    }

    public Vector3 GetTriggerContactPoint(Collider other)
    {
        Vector3 position = other != null ? other.bounds.center : transform.position;
        if (_surfaceColliders != null)
        {
            foreach (Collider surfaceCollider in _surfaceColliders)
            {
                if (surfaceCollider != null && surfaceCollider.enabled && !surfaceCollider.isTrigger)
                {
                    return surfaceCollider.ClosestPoint(position);
                }
            }
        }

        return transform.position;
    }

    public Vector3 GetTriggerSurfaceNormal(Collider other, Vector3 contactPoint)
    {
        Vector3 normal = contactPoint - transform.position;
        if (normal.sqrMagnitude < 0.0001f && other != null)
        {
            normal = other.bounds.center - transform.position;
        }

        if (normal.sqrMagnitude < 0.0001f)
        {
            normal = transform.up;
        }

        return normal.normalized;
    }
}
