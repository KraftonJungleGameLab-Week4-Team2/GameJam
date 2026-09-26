using UnityEngine;

// 실제 씬에 배치된 표면 오브젝트와 SurfaceProfile을 연결
public class SurfaceInstance : MonoBehaviour
{
    [SerializeField] private SurfaceProfile _profile;
    [SerializeField] private Collider[] _surfaceColliders;

    public SurfaceProfile Profile
    {
        get
        {
            return _profile;
        }
    }

    void Awake()
    {
        ApplyPhysicsMaterial();
    }

    void Reset()
    {
        _surfaceColliders = GetComponentsInChildren<Collider>();
    }

    private void ApplyPhysicsMaterial()
    {
        if (_profile == null || _profile.PhysicsMaterial == null)
        {
            return;
        }

        foreach (Collider surfaceCollider in _surfaceColliders)
        {
            if (surfaceCollider == null)
            {
                continue;
            }

            surfaceCollider.sharedMaterial = _profile.PhysicsMaterial;
        }
    }
}