using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public partial class HorseRaceFirstPersonAIDriver : MonoBehaviour
{
    [SerializeField] private HorseRaceThirdPersonBehaviour horseRaceThirdPersonBehaviour;
    [SerializeField] private NavMeshAgent navMeshAgent;
    private int currentWayPointIndex = 0;
    private Vector3 currentTargetWayPoint;
    private Vector3 lastTargetDirection;
    private bool isFirstlap = true;
    
    private void FixedUpdate()
    {
        if (!horseRaceThirdPersonBehaviour.IsStart) return;
        UpdateSpeed();
        if (IfReachTarget())
        {
            ChangeTarget();
            isFirstlap = false;
        }
    }

    private void UpdateSpeed()
    {
        navMeshAgent.speed = horseRaceThirdPersonBehaviour.CurrentForwardSpeed;
        navMeshAgent.acceleration = horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData.Acceleration;
    }

    private void ChangeTarget()
    {
        currentWayPointIndex++;
        currentWayPointIndex %= horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData.PredefineWayPoints.Length;
        currentTargetWayPoint = horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData.PredefineWayPoints[currentWayPointIndex];
        var lastTargetDirectionIndex
            = (currentWayPointIndex - 1 +
               horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData.PredefineWayPoints.Length) %
              horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData.PredefineWayPoints.Length;
        lastTargetDirection = currentTargetWayPoint - horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData.PredefineWayPoints[lastTargetDirectionIndex];
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
