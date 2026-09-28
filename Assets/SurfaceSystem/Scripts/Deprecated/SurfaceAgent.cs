// using UnityEngine;

// // 표면과 상호작용하는 객체가 현재 밟고 있는 표면 정보를 관리
// public class SurfaceAgent : MonoBehaviour
// {
//     private SurfaceInstance _currentSurface;

//     public SurfaceInstance CurrentSurface
//     {
//         get
//         {
//             return _currentSurface;
//         }
//     }

//     // 플레이어의 지면 판정 결과로 현재 표면을 설정한다.
//     public void SetSurface(Collider surfaceCollider)
//     {
//         if (surfaceCollider == null)
//         {
//             _currentSurface = null;
//             return;
//         }

//         _currentSurface = surfaceCollider.GetComponentInParent<SurfaceInstance>();
//     }

//     // 공중으로 이동했을 때 현재 표면을 해제한다.
//     public void ClearSurface()
//     {
//         _currentSurface = null;
//     }

//     // 표면이 없으면 이동 배율은 1, 추가 효과는 꺼진 값을 반환한다.
//     public SurfaceModifiers GetModifiers(float currentSpeed = 0f, float baseMaxSpeed = 0f)
//     {
//         if (_currentSurface == null)
//         {
//             return SurfaceModifiers.Default;
//         }

//         return _currentSurface.Profile.GetModifiers(currentSpeed, baseMaxSpeed);
//     }
// }
