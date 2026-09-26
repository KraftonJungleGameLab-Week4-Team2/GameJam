using System;

// 표면에 의해 변경되는 이동 관련 배율 값을 모아둔 구조체
[Serializable]
public struct SurfaceMovementModifiers
{
    public float accelerationMultiplier;
    public float decelerationMultiplier;
    public float turningMultiplier;
    public float maxSpeedMultiplier;
    public float jumpMultiplier;

    public static SurfaceMovementModifiers Default
    {
        get
        {
            SurfaceMovementModifiers modifiers = new SurfaceMovementModifiers();

            modifiers.accelerationMultiplier = 1f;
            modifiers.decelerationMultiplier = 1f;
            modifiers.turningMultiplier = 1f;
            modifiers.maxSpeedMultiplier = 1f;
            modifiers.jumpMultiplier = 1f;

            return modifiers;
        }
    }
}


