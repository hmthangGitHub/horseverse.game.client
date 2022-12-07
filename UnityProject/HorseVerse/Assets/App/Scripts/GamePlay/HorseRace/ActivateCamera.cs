using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActivateCamera : MonoBehaviour
{
    public GameObject cameraContainer;
    public event Func<UniTask> OnBeginActivateCameraEvent = () => UniTask.CompletedTask;
    public event Func<UniTask> OnActivateCameraEvent = () => UniTask.CompletedTask;

    private CinemachineVirtualCamera[] cinemachineVirtualCameras;
    private CinemachineVirtualCamera[] CinemachineVirtualCameras => cinemachineVirtualCameras ??= 
        cameraContainer.GetComponentsInChildren<CinemachineVirtualCamera>(true);
    public void Activate()
    {
        OnActivateAsync().Forget();
    }

    private async UniTaskVoid OnActivateAsync()
    {
        var parent = this.transform.parent;
        var nextTrigger = this.transform.GetSiblingIndex();
        nextTrigger++;
        nextTrigger %= parent.childCount;

        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(i == nextTrigger);
        }

        await OnBeginActivateCameraEvent();
        var activateCameraIndex = this.transform.GetSiblingIndex() + 1 % transform.parent.childCount; 
        for (int i = 0; i < CinemachineVirtualCameras.Length; i++)
        {
            CinemachineVirtualCameras[i].gameObject.SetActive(i == activateCameraIndex);
        }
        OnActivateCameraEvent.Invoke().Forget();
    }
}