// OpenFracture 원본 절단 알고리즘. 출처와 변경 사항은 LICENSE.txt에 기록한다.
namespace SurfaceSystem.OpenFracture
{
using System;
using UnityEngine;

public static class Vector3Extensions
{
    // 
    // that the normal is pointing to
    //   - p: The point being checked
    //   - n: The normal of the plane
    //   - o: The origin of the plane
    public static bool IsAbovePlane(this Vector3 p, Vector3 n, Vector3 o)
    {
        return (n.x * (p.x - o.x) + n.y * (p.y - o.y) + n.z * (p.z - o.z)) >= 0;
    }
}
}

