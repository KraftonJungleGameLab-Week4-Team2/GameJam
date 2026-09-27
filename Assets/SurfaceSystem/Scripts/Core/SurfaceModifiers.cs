using System;

using UnityEngine;

// 표면 효과를 모은 결과다. 경과 시간이나 파괴 여부 같은 실행 상태는 저장하지 않는다.
[Serializable]
public struct SurfaceModifiers
{
    [Header("Movement")]
    public float accelerationMultiplier;
    public float decelerationMultiplier;
    public float turningMultiplier;
    public float maxSpeedMultiplier;
    public float jumpMultiplier;

    [Header("Bounce")]
    public bool bounceEnabled;
    public float bounceMultiplier;
    public float minimumImpactSpeed;
    public float maxBounceSpeed;
    public float groundedBounceSpeed;

    [Header("Damage and Knockback")]
    public float contactDamage;
    public float damagePerSecond;
    // 표면 바깥 방향으로 더할 속도다.
    public float knockbackSpeed;

    [Header("Sinking")]
    public float sinkDelay;
    // 표면 안쪽으로 내려가는 초당 거리다.
    public float sinkSpeed;

    [Header("Explosion")]
    public bool explosionEnabled;
    public float explosionDelay;
    public float explosionRadius;
    public float explosionForce;
    public float explosionDamage;

    [Header("Fracture")]
    public bool breakEnabled;
    // 충돌 충격량의 크기가 이 값 이상일 때 파괴 대상으로 판단한다.
    public float breakImpulseThreshold;

    public static SurfaceModifiers Default
    {
        get
        {
            SurfaceModifiers modifiers = new SurfaceModifiers();

            modifiers.accelerationMultiplier = 1f;
            modifiers.decelerationMultiplier = 1f;
            modifiers.turningMultiplier = 1f;
            modifiers.maxSpeedMultiplier = 1f;
            modifiers.jumpMultiplier = 1f;

            return modifiers;
        }
    }
}


