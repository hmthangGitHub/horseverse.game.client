using System;
using PathCreation;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
[ExecuteInEditMode]
public class TargetGenerator : MonoBehaviour
{
    [SerializeField] private PredefinePath predefinePath;
    public PredefinePath PredefinePath => predefinePath;
    public PathCreator SimplyPath => PredefinePath.SimplyPath;
    public float spacing = 2.5f;
    public float offset = 2.5f;
    public int numberOfAgents = 8;
    public Vector3 StartPosition => PredefinePath.StartPosition;
    public Quaternion StartRotation => PredefinePath.StartRotation;


    public Vector3[] GenerateRandomTargets()
    {
        var numberOfWayPoint = 10;
        return Enumerable.Range(0, numberOfWayPoint)
                         .Select(i => Mathf.Lerp(PredefinePath.StartTime, PredefinePath.EndTime,  (float)i / (numberOfWayPoint - 1)))
                         .Select(x => FromTimeToPoint(x, 0.0f, SimplyPath))
                         .ToArray();
    }
    
    public Vector3[] GenerateRandomTargetsWithNoise()
    {
        var numberOfWayPoint = 40;
        var seed = UnityEngine.Random.value;
        return Enumerable.Range(0, numberOfWayPoint)
                         .Select(i => Mathf.Lerp(PredefinePath.StartTime, PredefinePath.EndTime,  (float)i / (numberOfWayPoint - 1)))
                         .Select(x =>
                         {
                             var lane = Mathf.Lerp(0, numberOfAgents - 1, Mathf.PerlinNoise(seed + x * (numberOfWayPoint - 1)  * 0.25f, 0.0f));
                             return FromTimeToPoint(x, GetOffsetFromLane(lane), SimplyPath);
                         })
                         .ToArray();
    }
    
    public static Vector3 FromTimeToPoint(float t, float offset, PathCreator pathCreator)
    {
        Vector3 rightV = Quaternion.Euler(0, pathCreator.path.GetRotation(t).eulerAngles.y, 0) * Vector3.right;
        return pathCreator.path.GetPointAtTime(t) + rightV.X0Z() * offset;
    }

    private (Vector3 position, Quaternion rotation, float time) FromTimeToPositionAndRotation(float t, float offset, float timeToFinish)
    {
        Vector3 rightV = Quaternion.Euler(0, SimplyPath.path.GetRotation(t).eulerAngles.y, 0) * Vector3.right;
        return (SimplyPath.path.GetPointAtTime(t) + rightV.X0Z() * offset , SimplyPath.path.GetRotation(t) * Quaternion.Euler(0, 180, 0), timeToFinish);
    }

    public float GetOffsetFromLane(float lane)
    {
        var start = spacing * ((numberOfAgents - 1) / 2.0f) + offset;
        return start - spacing * lane;
    }
    
    private float GetOffsetFromLane(int lane)
    {
        var start = spacing * ((numberOfAgents - 1) / 2.0f) + offset;
        return start - spacing * lane;
    }

    public ((Vector3 target, Quaternion, float time)[] targets,int finishIndex) GenerateTargets(RaceSegmentTime[] raceSegments)
    {
        var firstPoint = SimplyPath.bezierPath.GetPoint(PredefinePath.PredefinedWayPointIndices.First());
        var tFirst = SimplyPath.path.GetClosestTimeOnPath(firstPoint);
        
        var lastPoint = SimplyPath.bezierPath.GetPoint(PredefinePath.PredefinedWayPointIndices.Last());
        var tLast = SimplyPath.path.GetClosestTimeOnPath(lastPoint);

        const int interpolatePointNumber = 7;
        
        var paddingTargets = raceSegments.SelectMany((x, i) =>
        {
            var previousPercentage = i == 0 ? 0 : raceSegments[i - 1].Percentage;
            var previousLane = i == 0 ? x.CurrentLane: raceSegments[i - 1].ToLane;
            var offsetFromLane = GetOffsetFromLane(x.ToLane - 1);
            var previousOffsetFromLane = GetOffsetFromLane(previousLane - 1);
            var previousTime = Mathf.Lerp(tFirst, tLast, previousPercentage);
            var time = Mathf.Lerp(tFirst, tLast, x.Percentage);
            var timeToFinish = x.Time;

            return Enumerable.Range(0, interpolatePointNumber + 1)
                             .Select(point =>
                             {
                                 var interpolateRatio = (1.0f / (interpolatePointNumber + 1));
                                 var interpolateTimeToFinish = timeToFinish * interpolateRatio;
                                 var interpolateOffset = Mathf.Lerp(previousOffsetFromLane, offsetFromLane, interpolateRatio * (point + 1));
                                 var interpolateTime = Mathf.Lerp(previousTime, time, interpolateRatio * (point + 1));
                                 return FromTimeToPositionAndRotation(interpolateTime, interpolateOffset, interpolateTimeToFinish);
                             });
        }).ToArray();
        return (paddingTargets, paddingTargets.Length - 1);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (var i = 0; i < numberOfAgents; i++)
        {
            var position = FromTimeToPoint(PredefinePath.StartTime, GetOffsetFromLane(i), PredefinePath.SimplyPath);
            GUI.color = Color.black;
            Handles.Label(position + Vector3.right * 1.0f, $"lane :{i}");
            Gizmos.DrawSphere(position, 0.5f);
        }
    }
#endif
}
