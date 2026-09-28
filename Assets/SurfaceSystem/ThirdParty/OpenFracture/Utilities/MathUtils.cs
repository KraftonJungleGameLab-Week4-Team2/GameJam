// OpenFracture 원본 절단 알고리즘. 출처와 변경 사항은 LICENSE.txt에 기록한다.
namespace SurfaceSystem.OpenFracture
{
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static bool IsQuadConvex(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        return LinesIntersectInternal(a1, a2, b1, b2, true);
    }
    public static bool LinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        return LinesIntersectInternal(a1, a2, b1, b2, false);
    }
    private static bool LinesIntersectInternal(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, bool includeSharedEndpoints)
    {
        Vector2 a12 = new Vector2(a2.x - a1.x, a2.y - a1.y);
        Vector2 b12 = new Vector2(b2.x - b1.x, b2.y - b1.y);
        
        // If any of the vertices are shared between the two diagonals,
        // the quad collapses into a triangle and is convex by default.
        if (a1 == b1 || a1 == b2 || a2 == b1 || a2 == b2)
        {
            return includeSharedEndpoints;
        }
        else
        {
            // Compute cross product between each point and the opposite diagonal
            // Look at sign of the Z component to see which side of line point is on
            float a1xb = (a1.x - b1.x) * b12.y - (a1.y - b1.y) * b12.x;
            float a2xb = (a2.x - b1.x) * b12.y - (a2.y - b1.y) * b12.x;
            float b1xa = (b1.x - a1.x) * a12.y - (b1.y - a1.y) * a12.x;
            float b2xa = (b2.x - a1.x) * a12.y - (b2.y - a1.y) * a12.x;

            // Check that the points for each diagonal lie on opposite sides of the other
            // diagonal. Quad is also convex if a1/a2 lie on b1->b2 (and vice versa) since
            // the shape collapses into a triangle (hence >= instead of >)
            return ((a1xb >= 0 && a2xb <= 0) || (a1xb <= 0 && a2xb >= 0)) &&
                   ((b1xa >= 0 && b2xa <= 0) || (b1xa <= 0 && b2xa >= 0));
        }
    }
    public static bool LinePlaneIntersection(Vector3 a,
                                             Vector3 b,
                                             Vector3 n,
                                             Vector3 p0,
                                             out Vector3 x,
                                             out float s)
    {
        // Initialize out params
        s = 0;
        x = Vector3.zero;

        // Handle degenerate cases
        if (a == b)
        {
            return false;
        }
        else if (n == Vector3.zero)
        {
            return false;
        }

        // `s` is the parameter for the line segment a -> b where 0.0 <= s <= 1.0
        s = Vector3.Dot(p0 - a, n) / Vector3.Dot(b - a, n);

        if (s >= 0 && s <= 1)
        {
            x = a + (b - a) * s;
            return true;
        }

        return false;
    }
    public static bool IsPointOnRightSideOfLine(Vector2 a, Vector2 b, Vector2 c)
    {
        // The <= is essential; if it is <, the whole thing falls apart
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) <= 0;
    }

}

}

