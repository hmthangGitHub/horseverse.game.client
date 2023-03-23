using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using UnityEngine;

public class PredefinePath : PredefinePathBase
{
    [SerializeField] private PathCreator simplyPath;
    [SerializeField] private int[] predefinedWayPointIndices;
    private float? direction;
    private float? endTime;
    private float? startTime;
    private Vector3? startPosition;
    private Quaternion? startRotation;
    private int[] PredefinedWayPointIndices => predefinedWayPointIndices;
    private PathCreator SimplyPath => simplyPath;
    public override Vector3 StartPosition => startPosition ??= simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0]);
    public override Quaternion StartRotation => startRotation ??= Quaternion.FromToRotation(Vector3.forward, simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[1]) 
                                                                                           - simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0]));
    public override float StartTime => startTime ??= simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices.First()));
    public override float EndTime => endTime ??= simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices.Last()));

    public override float Direction => direction ??= Math.Sign(simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[1])) - 
                                                               simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0])));

    public override float GetClosestTime(Vector3 worldPoint)
    {
        return SimplyPath.path.GetClosestTimeOnPath(worldPoint);
    }

    public override Quaternion GetRotation(float time)
    {
        return SimplyPath.path.GetRotation(time);
    }

    public override Vector3 GetPointAtTime(float time)
    {
        return SimplyPath.path.GetPointAtTime(time);
    }
}
