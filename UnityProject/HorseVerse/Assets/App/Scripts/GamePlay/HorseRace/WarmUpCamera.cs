using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmUpCamera : MonoBehaviour
{
    public event Action OnFinishWarmingUp = ActionUtility.EmptyAction.Instance;
    public CinemachineVirtualCamera vCamera;
    public Animator animator;

    public void SetTargetGroup(Transform targetGroup)
    {
        vCamera.Follow = targetGroup;
        vCamera.LookAt = targetGroup;
    }

    public void OnFinishWarmUpAnimation()
    {
        OnFinishWarmingUp.Invoke();
    }

    private void OnEnable()
    {
        string message = $"WarmUpCamera{UnityEngine.Random.Range(1, 6)}";
        Debug.Log(message);
        animator.Play(message, 0, 0);
    }
}
