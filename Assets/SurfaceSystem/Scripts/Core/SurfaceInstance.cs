using UnityEngine;

// 행성에 물리 재질을 연결하고 Unity 충돌 이벤트를 프로필에 전달한다.
public class SurfaceInstance : MonoBehaviour
{
    [Header("Surface")]
    [SerializeField] private SurfaceProfile _profile;
    [SerializeField] private Collider[] _surfaceColliders = new Collider[0];

    public SurfaceProfile Profile
    {
        get { return _profile; }
    }

    void Awake()
    {
        if (_surfaceColliders.Length == 0)
        {
            _surfaceColliders = GetComponentsInChildren<Collider>();
        }

        foreach (Collider surfaceCollider in _surfaceColliders)
        {
            surfaceCollider.sharedMaterial = _profile.PhysicsMaterial;
        }
    }

    void Reset()
    {
        _surfaceColliders = GetComponentsInChildren<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        _profile.ProcessCollision(collision, true);
    }

    void OnCollisionStay(Collision collision)
    {
        _profile.ProcessCollision(collision, false);
    }
}
