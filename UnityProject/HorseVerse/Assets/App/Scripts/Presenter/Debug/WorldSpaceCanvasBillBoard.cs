using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvasBillBoard : MonoBehaviour
{
    [SerializeField]
    public Transform cameraTransform;

    public Transform CameraTransform
    {
        get => cameraTransform;
        set => cameraTransform = value;
    }

    void Update()
    {
        if (CameraTransform == default) return;
        transform.rotation = this.CameraTransform.rotation;
    }
}
