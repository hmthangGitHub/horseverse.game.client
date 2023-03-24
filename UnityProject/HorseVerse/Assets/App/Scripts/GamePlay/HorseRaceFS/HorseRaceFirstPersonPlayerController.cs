using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

public partial class HorseRaceFirstPersonPlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private HorseRaceThirdPersonBehaviour horseRaceThirdPersonBehaviour;
    [SerializeField] private Transform horseTransform;

    private Vector3 lastFrame;
    private Vector3 lastVelocity;
    private float lastTime;
    
    public void MoveHorizontal(int direction)
    {
        if (!horseRaceThirdPersonBehaviour.IsStart) return;
        horseRaceThirdPersonBehaviour.HorizontalDirection += direction;
        horseRaceThirdPersonBehaviour.HorizontalDirection = Mathf.Clamp(horseRaceThirdPersonBehaviour.HorizontalDirection, -1, 1);
    }

    private void FixedUpdate()
    {
        if (!horseRaceThirdPersonBehaviour.IsStart) return;
        
        UpdateHorsePositionRotation();
    }

    private void UpdateHorsePositionRotation()
    {
        var time = horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefinePath.GetClosestTime(horseTransform.position);
        CalculateRotation(time);
        CalculateVelocity(time);
    }

    private void CalculateVelocity(float time)
    {
        var pointInCurve = TargetGenerator.FromTimeToPoint(time, 0.0f, horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefinePath);
        var toPosition = (horseTransform.position - pointInCurve);
        var isGoingToWall = Vector3.Dot(toPosition, horseTransform.right) * horseRaceThirdPersonBehaviour.HorizontalDirection > 0;

        var horizontalSpeed = Mathf.Abs(toPosition.magnitude) >= horseRaceThirdPersonBehaviour.OffsetRange && isGoingToWall
            ? 0
            : horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.HorseRaceThirdPersonStats.HorizontalSpeed;
        rigidBody.velocity = horseTransform.rotation *
                             new Vector3(horseRaceThirdPersonBehaviour.HorizontalDirection * horizontalSpeed, 0, horseRaceThirdPersonBehaviour.CurrentForwardSpeed).normalized * horseRaceThirdPersonBehaviour.CurrentForwardSpeed;
    }

    private void CalculateRotation(float time)
    {
        var eulerAnglesY = horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefinePath.GetRotation(time)
                                                        .eulerAngles.y;
        horseTransform.rotation = Quaternion.Euler(0, eulerAnglesY, 0);
    }

    public void Sprint()
    {
        horseRaceThirdPersonBehaviour.Sprint();
    }
}
