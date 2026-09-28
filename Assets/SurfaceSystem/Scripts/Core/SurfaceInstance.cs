using UnityEngine;

public class SurfaceInstance : MonoBehaviour
{
    [Header("Surface")]
    [SerializeField] private SurfaceProfile _profile;
    [SerializeField] private Collider[] _surfaceColliders = new Collider[0];

    public SurfaceProfile Profile
    {
        get { return _profile; }
    }

    private void Awake()
    {
        if (_surfaceColliders.Length == 0)
        {
            _surfaceColliders = GetComponentsInChildren<Collider>();
        }

        if (_profile == null)
        {
            return;
        }

        foreach (Collider surfaceCollider in _surfaceColliders)
        {
            surfaceCollider.sharedMaterial = _profile.PhysicsMaterial;
        }
    }

    private void Reset()
    {
        _surfaceColliders = GetComponentsInChildren<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_profile == null)
        {
            return;
        }

        _profile.ProcessEnter(this, collision);
        _profile.ProcessImpact(this, collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (_profile == null)
        {
            return;
        }

        _profile.ProcessStay(this, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_profile == null)
        {
            return;
        }

        _profile.ProcessExit(this, collision);
    }
}