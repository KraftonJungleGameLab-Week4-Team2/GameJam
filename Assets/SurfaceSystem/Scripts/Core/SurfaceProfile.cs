using UnityEngine;

// 표면의 조합을 저장하고 Effect를 호출한다. 재질별 조건이나 물리 계산은 Effect가 맡는다.
[CreateAssetMenu(fileName = "SurfaceProfile", menuName = "SurfaceSystem/Surface Profile")]
public class SurfaceProfile : ScriptableObject
{
    [Header("Surface Settings")]
    [SerializeField] private PhysicsMaterial _physicsMaterial;
    [SerializeField] private SurfaceEffect[] _effects = new SurfaceEffect[0];

    public PhysicsMaterial PhysicsMaterial
    {
        get { return _physicsMaterial; }
    }

    // 등록된 Effect가 이동과 상호작용 설정을 하나의 결과에 채운다.
    public SurfaceModifiers GetModifiers(float currentSpeed = 0f, float baseMaxSpeed = 0f)
    {
        SurfaceModifiers modifiers = SurfaceModifiers.Default;
        float normalizedSpeed = 0f;
        if (baseMaxSpeed > Mathf.Epsilon)
        {
            normalizedSpeed = Mathf.Clamp01(currentSpeed / baseMaxSpeed);
        }

        foreach (SurfaceEffect effect in _effects)
        {
            effect.Modify(ref modifiers, normalizedSpeed);
        }

        return modifiers;
    }

    // 충돌마다 설정을 한 번 계산하고, 첫 충돌 또는 접촉 유지 효과를 실행한다.
    public void ProcessCollision(SurfaceInstance surface, Collision collision, bool isFirstContact)
    {
        SurfaceModifiers modifiers = GetModifiers();
        foreach (SurfaceEffect effect in _effects)
        {
            if (isFirstContact)
            {
                effect.OnImpact(surface, collision, modifiers);
            }
            else
            {
                effect.OnStay(surface, collision, modifiers);
            }
        }
    }
}
