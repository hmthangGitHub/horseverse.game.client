using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraChangeTriggerer : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public float timeToChangeTarget = 5.0f;
    public float changeTargetTime = 0.0f;
    public bool updateChangingTarget = false;
    public ICinemachineCamera to;
    public bool changingFollow = true;
    public bool changingLookAt = true;
    public bool isLookingForTopHorse;

    public void RandomTarget(ICinemachineCamera to, ICinemachineCamera from)
    {
        changeTargetTime = 0.0f;
        if (to.VirtualCameraGameObject == this.gameObject)
        {
            updateChangingTarget = true;
            this.to = to;
            Change();
        }
        else
        {
            updateChangingTarget = false;
        }
    }

    private void Change()   
    {
        if(targetGroup?.m_Targets.Length > 0)
        {
            if (isLookingForTopHorse)
            {
                var target = targetGroup.m_Targets.OrderByDescending(x => x.target.GetComponent<HorseController>()?.CurrentRaceProgressWeight ?? 0).FirstOrDefault().target;
                if (changingFollow)
                {
                    to.Follow = target;
                }

                if (changingLookAt)
                {
                    to.LookAt = target;
                }
            }
            else
            {
                var target = targetGroup.m_Targets.RandomElement().target;
                if (changingFollow)
                {
                    to.Follow = target;
                }
                if (changingLookAt)
                {
                    to.LookAt = target;
                }
            }    
        }    
    }
}