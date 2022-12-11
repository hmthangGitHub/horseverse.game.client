using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceModeCameras : MonoBehaviour
{
    public CinemachineClearShot cinemachineClearShot;
    public GameObject vCameraContainer;
    public GameObject triggerContainer;
    public void SetHorseGroup(Transform targetGroup)
    {
        this.GetComponentsInChildren<CameraChangeTriggerer>(true).ToList().ForEach(x => x.TargetGroup = targetGroup);
    }

    private void Start()
    {
        var cinemachines = vCameraContainer.GetComponentsInChildren<CinemachineVirtualCamera>(true);
        cinemachines.ForEach(x => x.gameObject.SetActive(false));
        cinemachines.First().gameObject.SetActive(true);
    }
}
