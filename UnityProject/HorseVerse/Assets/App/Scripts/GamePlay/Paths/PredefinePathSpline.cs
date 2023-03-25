using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BezierSolution;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PredefinePathSpline : PredefinePathBase
{
    [SerializeField] private BezierSpline spline;
    [SerializeField] private int[] predefinedWayPointIndices;
    
    private float? direction;
    private float? endTime;
    private float? startTime;
    private Vector3? startPosition;
    private Quaternion? startRotation;
    private int[] PredefinedWayPointIndices => predefinedWayPointIndices;
    private BezierSpline Spline => spline;
    public override Vector3 StartPosition => startPosition ??= Spline[PredefinedWayPointIndices.First()].position;
    public override Quaternion StartRotation => startRotation ??= GetRotation(StartTime);
    public override float StartTime => startTime ??= GetClosestTime(StartPosition);
    public override float EndTime => endTime ??=  GetClosestTime(Spline[PredefinedWayPointIndices.Last()].position);

    public override float Direction => direction ??= Math.Sign(GetClosestTime(Spline[PredefinedWayPointIndices[1]].position) - StartTime); 

    public override float GetClosestTime(Vector3 worldPoint)
    {
        spline.FindNearestPointTo(worldPoint, out var normalizedT);
        return normalizedT;
    }

    public override Quaternion GetRotation(float time)
    {
        return Quaternion.LookRotation(spline.GetTangent(time));
    }

    public override Vector3 GetPointAtTime(float time)
    {
        return Spline.GetPoint(time);
    }
}
