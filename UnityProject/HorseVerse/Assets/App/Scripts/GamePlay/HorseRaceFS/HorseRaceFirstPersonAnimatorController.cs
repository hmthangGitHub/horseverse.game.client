using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceFirstPersonAnimatorController : MonoBehaviour
{
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal"); 
    private static readonly int SpeedHash = Animator.StringToHash("Speed"); 
    [SerializeField] private Animator animator;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private HorseRaceThirdPersonBehaviour horseRaceThirdPersonBehaviour;
    
    private float animationHorizontalSpeed;
    private float horizontalVelocity;
    private float forwardAnimationVelocity;
    private float animationForwardSpeed;

    private void SetHorizontalDirection(float horizontalDirection)
    {
        animationHorizontalSpeed = Mathf.SmoothDamp(animationHorizontalSpeed, horizontalDirection, ref horizontalVelocity, smoothTime);
        animator.SetFloat(HorizontalHash, animationHorizontalSpeed);
    }
    
    private void SetSpeed(float speed)
    {
        animationForwardSpeed = Mathf.SmoothDamp(animationForwardSpeed, speed, ref forwardAnimationVelocity, smoothTime);
        animator.SetFloat(SpeedHash, animationForwardSpeed);
    }

    private void LateUpdate()
    {
        if (!horseRaceThirdPersonBehaviour.IsStart) return;
        SetSpeed(Mathf.InverseLerp(0.0f, horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y, horseRaceThirdPersonBehaviour.CurrentForwardSpeed));
        SetHorizontalDirection(horseRaceThirdPersonBehaviour.HorizontalDirection);
    }
}
