using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public event Action OnSkipFreeCamera = ActionUtility.EmptyAction.Instance;

    public void EndCameraAnimation()
    {
        OnSkipFreeCamera.Invoke();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnSkipFreeCamera.Invoke();
        }
    }
}
