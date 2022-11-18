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
        var firstPoint = SimplyPath.bezierPath.GetPoint(predefinePath.PredefinedWayPointIndices.First());
        var tFirst = SimplyPath.path.GetClosestTimeOnPath(firstPoint);
        
        var lastPoint = SimplyPath.bezierPath.GetPoint(predefinePath.PredefinedWayPointIndices.Last());
        var tLast = SimplyPath.path.GetClosestTimeOnPath(lastPoint);
        
        var paddingTargets = raceSegments.Select(x => (FromTimeToPoint(  Mathf.Lerp(tFirst, tLast, x.percentage), GetOffsetFromLane(x.toLane)), x.time)).ToArray();
        return (paddingTargets, raceSegments.Length - 1);
    }
}
