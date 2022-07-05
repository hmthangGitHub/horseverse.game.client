using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmUpCamera : MonoBehaviour
{
    public event Action OnFinishWarmingUp = ActionUtility.EmptyAction.Instance;
    public CinemachineVirtualCamera vCamera;

    public void SetTargetGroup(Transform targetGroup)
    {
        vCamera.Follow = targetGroup;
        vCamera.LookAt = targetGroup;
    }

    public void OnFinishWarmUpAnimation()
    {
        OnFinishWarmingUp.Invoke();
    }
}
