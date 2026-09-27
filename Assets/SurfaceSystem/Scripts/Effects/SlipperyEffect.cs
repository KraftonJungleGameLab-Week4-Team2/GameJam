using UnityEngine;

// 미끄러운 표면에서 가속, 감속, 방향 전환 특성을 조정
[CreateAssetMenu(fileName = "SlipperyEffect", menuName = "SurfaceSystem/Effects/Slippery Effect")]
public class SlipperyEffect : SurfaceEffect
{
    [Header("Slippery Movement")]
    [SerializeField]
    private AnimationCurve _accelerationBySpeed = new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(0.4f, 0.6f), new Keyframe(1f, 1f));
    [SerializeField, Range(0f, 1f)]
    private float _decelerationMultiplier = 0.1f;
    [SerializeField, Range(0f, 1f)]
    private float _turningMultiplier = 0.15f;
    [SerializeField, Min(0f)]
    private float _maxSpeedMultiplier = 1f;

    // 속도 비율에 맞춰 가속과 미끄러운 이동 배율을 누적한다.
    public override void Modify(ref SurfaceModifiers modifiers, float normalizedSpeed)
    {
        float accelerationMultiplier = _accelerationBySpeed.Evaluate(normalizedSpeed);

        modifiers.accelerationMultiplier *= accelerationMultiplier;
        modifiers.decelerationMultiplier *= _decelerationMultiplier;
        modifiers.turningMultiplier *= _turningMultiplier;
        modifiers.maxSpeedMultiplier *= _maxSpeedMultiplier;
    }
}
