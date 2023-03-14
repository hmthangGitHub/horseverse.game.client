using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineUtils : MonoBehaviour
{
    // https://www.habrador.com/tutorials/interpolation/1-catmull-rom-splines/
    // http://www.iquilezles.org/www/articles/minispline/minispline.htm
    public static Vector3 GetPoint(List<Vector3> points, int firstPoint, float t)
    {
        var p0 = points[firstPoint];
        var p1 = points[(firstPoint + 1) % points.Count];
        var p2 = points[(firstPoint + 2) % points.Count];
        var p3 = points[(firstPoint + 3) % points.Count];

        var a = 2f * p1;
        var b = p2 - p0;
        var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        var d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + b * t + c * t * t + d * t * t * t);
    }

    public static Vector3 GetTangent(List<Vector3> points, int firstPoint, float t)
    {
        var p0 = GetPoint(points, firstPoint, t == 1f ? 0.99999f : t);
        var p1 = GetPoint(points, firstPoint, t == 1f ? 1f : t + 0.00001f);
        return (p1 - p0).normalized;
    }
}
