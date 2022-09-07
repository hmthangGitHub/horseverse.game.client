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
        //  cinemachineClearShot.Follow = targetGroup.transform;
        //cinemachineClearShot.LookAt = targetGroup.transform;
        this.GetComponentsInChildren<CameraChangeTriggerer>(true).ToList().ForEach(x => x.targetGroup = targetGroup);
    }
}
