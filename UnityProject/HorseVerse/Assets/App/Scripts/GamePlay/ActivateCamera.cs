using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCamera : MonoBehaviour
{
    public GameObject activateCamera;
    public CinemachineVirtualCamera excludeVCam;

    public void Activate()
    {
        foreach (Transform item in activateCamera.transform.parent)
        {
            item.gameObject.SetActive(item.gameObject == activateCamera);
        }
    }    
}
