using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvasBillBoard : MonoBehaviour
{
    public Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.rotation = this.cameraTransform.rotation;
    }
}
