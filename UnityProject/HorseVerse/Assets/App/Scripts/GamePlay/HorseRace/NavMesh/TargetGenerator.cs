using PathCreation;
using System.Linq;
using UnityEngine;

public class TargetGenerator : MonoBehaviour
{
    [SerializeField] private PredefinePath predefinePath;
    public PathCreator SimplyPath => predefinePath.SimplyPath;
    public float spacing = 2.5f;
    public int numberOfAgents = 8;
    public Vector3 StartPosition => predefinePath.StartPosition;
    public Quaternion StartRotation => predefinePath.StartRotation;

    public Vector3[] GenerateRandomTargets()
    {
        return Enumerable.Range(0, 11)
            .Select(i => i * 0.1f + Random.Range(-0.05f, 0.05f))
            .Select(x => FromTimeToPoint(x, 0))
            .ToArray();
    }

    private Vector3 FromTimeToPoint(float t, float offset)
    {
        Vector3 rightV = Quaternion.Euler(0, SimplyPath.path.GetRotation(t).eulerAngles.y, 0) * Vector3.right;
        return SimplyPath.path.GetPointAtTime(t) + rightV.X0Z() * offset;
    }

    private float GetOffsetFromLane(int lane)
    {
        var start = -spacing * ((numberOfAgents - 1) / 2.0f);
        return start + spacing * lane;
    }

    public ((Vector3 target, float time)[] targets,int finishIndex) GenerateTargets(RaceSegment[] raceSegments)
    {
        var paddingRaceSegments = raceSegments.Concat(new[]
        {
            new RaceSegment()
            {
                id = raceSegments.Length,
                currentLane = raceSegments[raceSegments.Length - 1].toLane,
                toLane = raceSegments[raceSegments.Length - 1].toLane,
                waypoints = GetPaddingWayPoints()
            }
        }).ToArray();

        var normalizePaths = FromSegmentToNormalizePath(paddingRaceSegments);
        var paddingTargets = paddingRaceSegments.SelectMany((x, segmentIndex) =>
        {
            var startOffset = GetOffsetFromLane(x.currentLane);
            var endOffset = GetOffsetFromLane(x.toLane);
            var startChangeLaneSegmentIndex = Random.Range(0, x.waypoints.Length - 1);
            var totalWayPointToChangeLane = (float)(x.waypoints.Length - 1) - startChangeLaneSegmentIndex;
            return x.waypoints.Select((w, wIndex) =>
            {
                var t = Mathf.Lerp(normalizePaths[segmentIndex].x, normalizePaths[segmentIndex].y, w.percentage);
                var offset = Mathf.Lerp(startOffset, endOffset,(wIndex - startChangeLaneSegmentIndex) / totalWayPointToChangeLane);
                return (FromTimeToPoint(t, offset), w.time);
            });
        }).ToArray();
        return (paddingTargets, raceSegments.SelectMany(x => x.waypoints).Count() - 1);
    }

    private WayPoints[] GetPaddingWayPoints()
    {
        var paddingWayPointCount = 5;
        return Enumerable.Range(0, paddingWayPointCount)
            .Select((x, i) => new WayPoints()
            {
                percentage = (float)i / (paddingWayPointCount - 1),
                time = 10.0f
            }).ToArray();
    }

    private Vector2[] FromSegmentToNormalizePath(RaceSegment[] segment)
    {
        if(segment.Length < predefinePath.PredefinedWayPointIndices.Length)
        {
            throw new System.Exception("Segment number larger than predefine one");
        }

        return segment.Select((x, i) =>
        {
            if(i == segment.Length - 1)
            {
                return new Vector2(GetTimeAtIndex(i), 1.0f * Mathf.Sign( GetTimeAtIndex(i) - GetTimeAtIndex(i - 1) ) + GetTimeAtIndex(0));
            }
            else
            {
                return new Vector2(GetTimeAtIndex(i), GetTimeAtIndex(i + 1));
            }
        }).ToArray();
    }

    private float GetTimeAtIndex(int i)
    {
         return SimplyPath.path.GetClosestTimeOnPath(SimplyPath.bezierPath.GetPoint(predefinePath.PredefinedWayPointIndices[i]));
    }
}
