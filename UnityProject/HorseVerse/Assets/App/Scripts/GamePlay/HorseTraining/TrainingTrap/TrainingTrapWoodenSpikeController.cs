using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTrapWoodenSpikeController : MonoBehaviour
{
    [SerializeField] GameObject RotateObject;
    [SerializeField] public float RotateSpeed = 10;

    private bool isReady = false;
    public bool IsReady { get => isReady; set { isReady = value; } }

    private void Update()
    {
        if (IsReady)
        {
            UpdateRotate();
        }
    }

    private void UpdateRotate()
    {
        RotateObject.transform.RotateAround(RotateObject.transform.position, RotateObject.transform.up, RotateSpeed * Time.deltaTime);
    }
}
