using UnityEngine;

// 각 효과가 설정 계산, 적용 대상 판정, 실제 동작을 맡는다. 대상별 실행 상태는 에셋에 저장하지 않는다.
public abstract class SurfaceEffect : ScriptableObject
{
    // normalizedSpeed는 기본 최고 속도 대비 현재 속도의 0~1 값이다.
    public virtual void Modify(ref SurfaceModifiers modifiers, float normalizedSpeed) {}

    // Collision에서 필요한 정보를 읽고, 각 효과가 적용 가능한 충돌인지 판단한다.
    public virtual void OnImpact(SurfaceInstance surface, Collision collision, SurfaceModifiers modifiers) {}

    // 접촉이 유지되는 동안 필요한 효과를 적용한다.
    public virtual void OnStay(SurfaceInstance surface, Collision collision, SurfaceModifiers modifiers) {}
}
