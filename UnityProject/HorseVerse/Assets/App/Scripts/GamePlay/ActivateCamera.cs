using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCamera : MonoBehaviour
{
    public GameObject activateCamera;

    public void Activate()
    {
        foreach (Transform item in activateCamera.transform.parent)
        {
            item.gameObject.SetActive(item.gameObject == activateCamera);
        }

        var parent = this.transform.parent;
        var nextTrigger = this.transform.GetSiblingIndex();
        nextTrigger++;
        nextTrigger %= parent.childCount;

        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(i == nextTrigger);
        }
    }
}
