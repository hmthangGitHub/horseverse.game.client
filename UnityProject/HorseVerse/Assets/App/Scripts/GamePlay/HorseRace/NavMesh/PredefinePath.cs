using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using UnityEngine;

public class PredefinePath : MonoBehaviour
{
    [SerializeField] private PathCreator simplyPath;
    [SerializeField] private int[] predefinedWayPointIndices;
    private float? direction;
    private float? endTime;
    private float? startTime;

    public int[] PredefinedWayPointIndices => predefinedWayPointIndices;
    public PathCreator SimplyPath => simplyPath;
    public Vector3 StartPosition => simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0]);
    public Quaternion StartRotation => Quaternion.FromToRotation(Vector3.forward, simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[1]) 
                                                   - simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0]));
    public float StartTime => startTime ??= simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices.First()));
    public float EndTime => endTime ??= simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices.Last()));

    public float Direction => direction ??= Math.Sign(simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[1])) - 
        simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0])));
}
