using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public partial class HorseRaceFirstPersonAIDriver : MonoBehaviour
{
    [SerializeField] private HorseRaceFirstPersonController horseRaceFirstPersonController;
    [SerializeField] private NavMeshAgent navMeshAgent;
    private int currentWayPointIndex = 0;
    private Vector3 currentTargetWayPoint;
    private Vector3 lastTargetDirection;
    private bool isFirstlap = true;
    
    private void FixedUpdate()
    {
        if (!horseRaceFirstPersonController.IsStart) return;
        if (IfReachTarget())
        {
            ChangeTarget();
            isFirstlap = false;
        }
    }
    
    private void ChangeTarget()
    {
        currentWayPointIndex++;
        currentWayPointIndex %= horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints.Length;
        currentTargetWayPoint = horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints[currentWayPointIndex];
        var lastTargetDirectionIndex
            = (currentWayPointIndex - 1 +
               horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints.Length) %
              horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints.Length;
        lastTargetDirection = currentTargetWayPoint - horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints[lastTargetDirectionIndex];
        navMeshAgent.destination = currentTargetWayPoint;
    }
    
    private bool IfReachTarget()
    {
        return (isFirstlap && currentWayPointIndex == 0) || Vector3.Dot(transform.position - currentTargetWayPoint, lastTargetDirection) >= 0.0f || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    private float DistanceToCurrentTarget()
    {
        return navMeshAgent.remainingDistance;
    }
}
