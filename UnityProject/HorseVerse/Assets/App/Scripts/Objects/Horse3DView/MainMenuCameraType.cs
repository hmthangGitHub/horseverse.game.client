using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainMenuCameraType : MonoBehaviour
{
    public enum CameraType
    {
        MainMenu,
        MainMenuZoomOut,
        Stable,
        StableDetail
    }

    public CinemachineVirtualCamera[] vCams;

    public void SetCameraType(CameraType cameraType)
    {
        vCams.ForEach((x, i) => x.Priority = (int)cameraType == i ? 11 : 10);
    }
}
