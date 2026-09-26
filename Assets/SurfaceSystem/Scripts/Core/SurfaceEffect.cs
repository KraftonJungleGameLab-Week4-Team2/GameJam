using UnityEngine;

// 모든 표면 효과가 상속받는 공통 기반 클래스
public abstract class SurfaceEffect : ScriptableObject
{
    public virtual void ModifyMovement(ref SurfaceMovementModifiers modifiers, SurfaceMovementContext context) {}
}