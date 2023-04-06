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
    [SerializeField] private Rigidbody rigidbody;
    private int currentWayPointIndex = 0;
    private Vector3 currentTargetWayPoint;
    private Vector3 lastTargetDirection;
    private bool isFirstlap = true;

    private void FixedUpdate()
    {
        if (!horseRaceThirdPersonBehaviour.IsStart) return;
        navMeshAgent.updatePosition = false;
        UpdateVelocity();
        UpdateRotation();
        UpdateTarget();
        SprintIfAvailable();
    }

    private void SprintIfAvailable()
    {
        if (horseRaceThirdPersonBehaviour.IsAbleToSprint)
        {
            horseRaceThirdPersonBehaviour.Sprint();
        }
    }

    private void UpdateRotation()
    {
        horseRaceThirdPersonBehaviour.transform.rotation = transform.rotation;
    }

    private void UpdateTarget()
    {
        if (IfReachTarget())
        {
            ChangeTarget();
            isFirstlap = false;
        }
    }

    private void UpdateVelocity()
    {
        UpdateSpeed();
        rigidbody.velocity = navMeshAgent.velocity;
    }

    private void UpdateSpeed()
    {
        navMeshAgent.speed = horseRaceThirdPersonBehaviour.CurrentForwardSpeed;
        // navMeshAgent.acceleration = horseRaceThirdPersonBehaviour.CurrentAcceleration;
    }

    private void ChangeTarget()
    {
        currentWayPointIndex++;
        currentWayPointIndex %= horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefineWayPoints.Length;
        currentTargetWayPoint = horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefineWayPoints[currentWayPointIndex];
        var lastTargetDirectionIndex
            = (currentWayPointIndex - 1 +
               horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefineWayPoints.Length) %
              horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefineWayPoints.Length;
        lastTargetDirection = currentTargetWayPoint - horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefineWayPoints[lastTargetDirectionIndex];
        navMeshAgent.destination = currentTargetWayPoint;
    }
    
    private bool IfReachTarget()
    {
        return (isFirstlap && currentWayPointIndex == 0) || Vector3.Dot(transform.position - currentTargetWayPoint, lastTargetDirection) >= 0.0f || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }
}
