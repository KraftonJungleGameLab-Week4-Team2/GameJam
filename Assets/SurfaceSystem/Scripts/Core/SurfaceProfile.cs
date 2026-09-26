using UnityEngine;

// 하나의 표면이 사용할 PhysicsMaterial과 여러 SurfaceEffect를 정의
[CreateAssetMenu(fileName = "SurfaceProfile", menuName = "SurfaceSystem/Surface Profile")]
public class SurfaceProfile : ScriptableObject
{
    [SerializeField] private PhysicsMaterial _physicsMaterial;
    [SerializeField] private SurfaceEffect[] _effects;

    public PhysicsMaterial PhysicsMaterial
    {
        get
        {
            return _physicsMaterial;
        }
    }

    public SurfaceMovementModifiers GetMovementModifiers(float currentSpeed, float baseMaxSpeed)
    {
        SurfaceMovementModifiers modifiers = SurfaceMovementModifiers.Default;
        SurfaceMovementContext context = new SurfaceMovementContext(currentSpeed, baseMaxSpeed);

        foreach (SurfaceEffect effect in _effects)
        {
            if (effect == null)
            {
                continue;
            }

            effect.ModifyMovement(ref modifiers, context);
        }

        return modifiers;
    }
}