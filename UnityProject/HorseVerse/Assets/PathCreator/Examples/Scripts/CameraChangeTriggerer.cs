using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeTriggerer : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public float timeToChangeTarget = 5.0f;
    public float changeTargetTime = 0.0f;
    public bool updateChangingTarget = false;
    public ICinemachineCamera to;

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
        int index = UnityEngine.Random.Range(0, targetGroup.m_Targets.Length);
        if (to.Follow != null)
        {
            to.Follow = targetGroup.m_Targets[index].target;
        }

        if (to.LookAt != null)
        {
            to.LookAt = targetGroup.m_Targets[index].target;
        }
    }

    public void Update()
    {
        if (updateChangingTarget)
        {
            changeTargetTime += Time.deltaTime;
            if (changeTargetTime >= timeToChangeTarget)
            {
                changeTargetTime = 0.0f;
                Change();
            }
        }
    }
}