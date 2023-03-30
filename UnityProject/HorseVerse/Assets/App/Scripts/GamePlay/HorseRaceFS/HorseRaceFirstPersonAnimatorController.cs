using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceFirstPersonAnimatorController : MonoBehaviour
{
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal"); 
    private static readonly int SpeedHash = Animator.StringToHash("Speed"); 
    private static readonly int AnimationSpeedHash = Animator.StringToHash("AnimationSpeed"); 
    [SerializeField] private Animator animator;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private HorseRaceThirdPersonBehaviour horseRaceThirdPersonBehaviour;
    
    private float animationHorizontalSpeed;
    private float horizontalVelocity;
    private float forwardAnimationVelocity;
    private float animationForwardSpeed;
    private float animationSpeed;
    private float animationSpeedVelocity;

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
        if (horseRaceThirdPersonBehaviour.CurrentForwardSpeed > horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData
                .HorseRaceThirdPersonStats.ForwardSpeedRange.y)
        {
            SetAnimationSpeed(horseRaceThirdPersonBehaviour.CurrentForwardSpeed/ horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData
                .HorseRaceThirdPersonStats.ForwardSpeedRange.y);
        }
        else
        {
            SetAnimationSpeed(1);
        }
        SetHorizontalDirection(horseRaceThirdPersonBehaviour.HorizontalDirection);
    }

    private void SetAnimationSpeed(float speed)
    {
        speed = Mathf.Min(speed, 1.5f);
        animationSpeed = Mathf.SmoothDamp(animationSpeed, speed, ref animationSpeedVelocity, smoothTime);
        animator.SetFloat(AnimationSpeedHash, animationSpeed);
    }

    public void Reset()
    {
        animator = horseRaceThirdPersonBehaviour.GetComponentInChildren<Animator>();
    }
}
