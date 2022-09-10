using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActivateCamera : MonoBehaviour
{
    public event Func<UniTask> OnBeginActivateCameraEvent = () => UniTask.CompletedTask;
    public event Func<UniTask> OnActivateCameraEvent = () => UniTask.CompletedTask;

    public CinemachineVirtualCamera activateCamera;
    private CinemachineVirtualCamera[] cinemachineVirtualCameras;
    private CinemachineVirtualCamera[] CinemachineVirtualCameras => cinemachineVirtualCameras ??= 
        activateCamera.transform.parent.GetComponentsInChildren<CinemachineVirtualCamera>(true);
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
        foreach (var item in CinemachineVirtualCameras)
        {
            item.gameObject.SetActive(item == activateCamera);
        }
        OnActivateCameraEvent.Invoke().Forget();
    }
}