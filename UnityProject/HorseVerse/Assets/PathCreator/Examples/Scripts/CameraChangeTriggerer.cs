using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraChangeTriggerer : MonoBehaviour
{
    public Transform TargetGroup
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
                cinemachineVirtualCamera.LookAt = value.transform;
            }
        }
    }

    public bool changingFollow = true;
    public bool changingLookAt = true;
    [SerializeField] private Transform targetGroup;
}