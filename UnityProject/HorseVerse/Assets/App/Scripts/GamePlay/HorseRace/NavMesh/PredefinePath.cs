using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class PredefinePath : MonoBehaviour
{
    [SerializeField] private PathCreation.PathCreator simplyPath;
    [SerializeField] private int[] predefinedWayPointIndices;
    public int[] PredefinedWayPointIndices => predefinedWayPointIndices;
    public PathCreator SimplyPath { get => simplyPath; set => simplyPath = value; }
    public Vector3 StartPosition => simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0]);
    public Quaternion StartRotation => Quaternion.FromToRotation(Vector3.forward, simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[1]) 
                                                   - simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0]));
    public float StartTime => simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0]));

    private float? direction;
    public float Direction => direction ??= Math.Sign(simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[1])) - 
        simplyPath.path.GetClosestTimeOnPath(simplyPath.bezierPath.GetPoint(PredefinedWayPointIndices[0])));
}
