using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class CinemachineFollower : MonoBehaviour
{
    public CinemachinePathBase smoothPath;
    public CinemachineVirtualCamera vCam;
    public Vector3 rotationOffset;
    public CinemachineTrackedDolly trackedDolly;

    void LateUpdate()
    {
        var rotation = smoothPath.EvaluateOrientationAtUnit(trackedDolly.m_PathPosition,  trackedDolly.m_PositionUnits);
        // this.transform.rotation = Quaternion.Euler(rotation.eulerAngles + rotationOffset);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(rotation.eulerAngles + rotationOffset),
            Time.deltaTime * 10.0f);
    }

    void Reset()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        trackedDolly = vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        smoothPath = trackedDolly.m_Path;
    }
}
