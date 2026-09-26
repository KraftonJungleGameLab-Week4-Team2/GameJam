using UnityEngine;

// 표면 효과 계산에 필요한 현재 이동 상태 정보를 전달
public readonly struct SurfaceMovementContext
{
    private readonly float _currentSpeed;
    private readonly float _baseMaxSpeed;

    public float CurrentSpeed => _currentSpeed;
    public float BaseMaxSpeed => _baseMaxSpeed;

    public float NormalizedSpeed
    {
        get
        {
            if (_baseMaxSpeed <= Mathf.Epsilon)
            {
                return 0f;
            }

            return Mathf.Clamp01(_currentSpeed / _baseMaxSpeed);
        }
    }

    public SurfaceMovementContext(float currentSpeed, float baseMaxSpeed)
    {
        _currentSpeed = currentSpeed;
        _baseMaxSpeed = baseMaxSpeed;
    }
}