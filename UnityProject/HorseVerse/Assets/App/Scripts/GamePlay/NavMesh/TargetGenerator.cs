using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetGenerator : MonoBehaviour
{
    [SerializeField] private PathCreation.PathCreator simplyPath;
    [SerializeField] private int[] predefinedWayPointIndices;
    public int[] PredefinedWayPointIndices { get => predefinedWayPointIndices; set => predefinedWayPointIndices = value; }
    public PathCreator SimplyPath { get => simplyPath; set => simplyPath = value; }
    public float spacing = 2.5f;
    public int numberOfAgents = 8;

    public Vector3[] GenerateRandomTargets()
    {
        return Enumerable.Range(0, 11)
            .Select(i => i * 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f))
            .Select(x => FromTimeToPoint(x, 0))
            .ToArray();
    }

    public Vector3 FromTimeToPoint(float t, float offset)
    {
        Vector3 rightV = Quaternion.Euler(0, SimplyPath.path.GetRotation(t).eulerAngles.y, 0) * Vector3.right;
        return SimplyPath.path.GetPointAtTime(t) + rightV.X0Z() * offset;
    }

    private float GetOffsetFromLane(int lane)
    {
        var start = -spacing * ((float)(numberOfAgents - 1) / 2.0f);
        return start + spacing * lane;
    }

    public (Vector3 target, float time)[] GenerateTargets(RaceSegment[] raceSegments)
    {
        var normalizePaths = FromSegmentToNormalizePath(raceSegments);
        return raceSegments.SelectMany((x, segmentIndex) =>
        {
            var startOffset = GetOffsetFromLane(x.currentLane);
            var endOffset = GetOffsetFromLane(x.toLane);
            var startChangeLaneSegmentIndex = UnityEngine.Random.Range(0, x.waypoints.Length - 1);
            var totalWayPointToChangeLane = (float)(x.waypoints.Length - 1) - startChangeLaneSegmentIndex;
            return x.waypoints.Select((w, wIndex) =>
            {
                var t = Mathf.Lerp(normalizePaths[segmentIndex].x, normalizePaths[segmentIndex].y, w.percentage);
                var offset = Mathf.Lerp(startOffset, endOffset, (wIndex - startChangeLaneSegmentIndex) / totalWayPointToChangeLane);
                return (FromTimeToPoint(t, offset), w.time);
            });
        }).ToArray();
    }

    private Vector2[] FromSegmentToNormalizePath(RaceSegment[] segment)
    {
        if(segment.Length < PredefinedWayPointIndices.Length)
        {
            throw new System.Exception("Segment number larger than predefine one");
        }

        return segment.Select((x, i) =>
        {
            if(i == segment.Length - 1)
            {
                return new Vector2(GetTimeAtIndex(i), 1.0f);
            }
            else
            {
                return new Vector2(GetTimeAtIndex(i), GetTimeAtIndex(i + 1));
            }
        }).ToArray();
    }

    private float GetTimeAtIndex(int i)
    {
         return simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[i]));
    }
}
