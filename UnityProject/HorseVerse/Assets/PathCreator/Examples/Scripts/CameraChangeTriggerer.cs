using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraChangeTriggerer : MonoBehaviour
{
    public CinemachineTargetGroup TargetGroup
    {
        get => targetGroup;
        set
        {
            targetGroup = value;
            var cinemachineVirtualCamera = this.GetComponent<CinemachineVirtualCamera>();
            if (changingFollow)
            {
                cinemachineVirtualCamera.Follow = value.transform;
            }

            if (changingLookAt)
            {
                cinemachineVirtualCamera.LookAt = cinemachineVirtualCamera.Follow;
            }
        }
    }

    public float timeToChangeTarget = 5.0f;
    public float changeTargetTime = 0.0f;
    public bool updateChangingTarget = false;
    public ICinemachineCamera to;
    public bool changingFollow = true;
    public bool changingLookAt = true;
    public bool isLookingForTopHorse;
    [SerializeField] private CinemachineTargetGroup targetGroup;
}