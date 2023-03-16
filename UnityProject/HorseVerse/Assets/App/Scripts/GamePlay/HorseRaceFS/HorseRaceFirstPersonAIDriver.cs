using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public partial class HorseRaceFirstPersonAIDriver : MonoBehaviour
{
    [SerializeField] private HorseRaceFirstPersonController horseRaceFirstPersonController;
    private int currentWayPointIndex = 0;
    private Vector3 currentTargetWayPoint;
    private Vector3 lastTargetDirection;
    private float lastOffset;
    private float targetOffset;
    
    private void FixedUpdate()
    {
        if (!horseRaceFirstPersonController.IsStart) return;
        if (IfReachTarget())
        {
            ChangeTarget();
        }
        ToTarget();
    }

    private void ToTarget()
    {
        var currentOffset = GetOffsetFromPath(this.transform.position);
    }

    private void ChangeTarget()
    {
        currentWayPointIndex++;
        currentTargetWayPoint = horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints[currentWayPointIndex];
        lastTargetDirection = currentTargetWayPoint - horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints[currentWayPointIndex - 1];
        lastOffset = GetOffsetFromPath(this.transform.position);
        targetOffset = GetOffsetFromPath(currentTargetWayPoint);
    }

    private float GetOffsetFromPath(Vector3 worldPosition)
    {
        var time= horseRaceFirstPersonController.PredefinePath.SimplyPath.path.GetClosestTimeOnPath(worldPosition);
        var pointOnPath = horseRaceFirstPersonController.PredefinePath.SimplyPath.path.GetPointAtTime(time);
        var eulerAnglesY = horseRaceFirstPersonController.PredefinePath.SimplyPath.path.GetRotation(time)
                                                         .eulerAngles.y + 180 * horseRaceFirstPersonController.PredefinePath.Direction;
        var right = Quaternion.Euler(0, eulerAnglesY, 0) * Vector3.right;
        return Vector3.Dot((transform.position - pointOnPath), right);
    }

    private bool IfReachTarget()
    {
        return currentWayPointIndex == 0 || Vector3.Dot(transform.position - currentTargetWayPoint, lastTargetDirection) >= 0.0f;
    }
}
