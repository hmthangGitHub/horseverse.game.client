using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControlPointsAuthoring : MonoBehaviour
{
    public Transform[] points;

    void OnDrawGizmos()
    {
        if (points == default) return;
        var pointCount = points.Length;
        if (pointCount == 0) return;

        for (int i = 0; i < pointCount; i++)
        {
            var previousPosition = (i - 1 < 0) ? points[pointCount - 1].position : points[i - 1].position;
            var startPosition = points[i].position;
            var endPosition = points[(i + 1) % pointCount].position;
            var nextPosition = points[(i + 2) % pointCount].position;

            var controlPoints = new Vector3[4] { previousPosition, startPosition, endPosition, nextPosition };

            Gizmos.color = Color.green;
            DrawCurve(controlPoints);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPosition, 0.4f);
        }
    }

    void DrawCurve(Vector3[] controlPoints)
    {
        var lineCount = 50;
        List<Vector3> points = controlPoints.ToList();
        for (int i = 1; i <= lineCount; i++)
        {
            var previousRatio = (i - 1) / (float)lineCount;
            var ratio = i / (float)lineCount;
            {
                var previousPosition = SplineUtils.GetPoint(points, 0, previousRatio);
                var currentPosition = SplineUtils.GetPoint(points, 0, ratio);
                Gizmos.DrawLine(previousPosition, currentPosition);
            }
        }
    }

    public Vector3[] getControlPoint()
    {
        return points.Select(x => x.position).ToArray();
    }
}
