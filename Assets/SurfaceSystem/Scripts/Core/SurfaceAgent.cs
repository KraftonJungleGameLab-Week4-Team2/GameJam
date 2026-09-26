using UnityEngine;

// 표면과 상호작용하는 객체가 현재 밟고 있는 표면 정보를 관리
public class SurfaceAgent : MonoBehaviour
{
    private SurfaceInstance _currentSurface;

    public SurfaceInstance CurrentSurface
    {
        get
        {
            return _currentSurface;
        }
    }

    public void SetSurface(Collider surfaceCollider)
    {
        if (surfaceCollider == null)
        {
            _currentSurface = null;
            return;
        }

        _currentSurface = surfaceCollider.GetComponentInParent<SurfaceInstance>();
    }

    public void ClearSurface()
    {
        _currentSurface = null;
    }

    public SurfaceMovementModifiers GetMovementModifiers(float currentSpeed, float baseMaxSpeed)
    {
        if (_currentSurface == null || _currentSurface.Profile == null)
        {
            return SurfaceMovementModifiers.Default;
        }

        return _currentSurface.Profile.GetMovementModifiers(currentSpeed, baseMaxSpeed);
    }
}