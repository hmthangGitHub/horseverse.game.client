using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraActiveHandler : MonoBehaviour
{
    public static Camera mainCamera;

    [SerializeField] Camera cam;

    private void Reset()
    {
        cam ??= this.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        mainCamera ??= this.cam;
    }

    private void OnDestroy()
    {
        if (mainCamera == this.cam) mainCamera = default;
    }

    public void ActiveThis()
    {
        mainCamera = this.cam;
    }
}
