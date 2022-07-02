using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceModeCameras : MonoBehaviour
{
    public CinemachineClearShot cinemachineClearShot;
    public void SetHorseGroup(CinemachineTargetGroup targetGroup)
    {
        cinemachineClearShot.LookAt = targetGroup.transform;
        this.GetComponentsInChildren<CameraChangeTriggerer>().ToList().ForEach(x => x.targetGroup = targetGroup);
    }

    public void EnterIntroCamera()
    {

    }
}
