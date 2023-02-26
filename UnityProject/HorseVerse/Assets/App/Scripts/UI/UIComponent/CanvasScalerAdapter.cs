using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteAlways]
public class CanvasScalerAdapter : MonoBehaviour
{
    [SerializeField]
    private CanvasScaler canvasScaler;
    private float lastScreenWidth = 0;
    private float lastScreenHeight = 0;
    
    private void Start()
    {
        OnScreenSizeChanged();
    }


    void Update()
    {
        if (Math.Abs(lastScreenWidth - Screen.width) > Mathf.Epsilon || Math.Abs(lastScreenHeight - Screen.height) > Mathf.Epsilon)
        {
            OnScreenSizeChanged();
        }
    }

    private void OnScreenSizeChanged()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        var matchWidthOrHeight = lastScreenWidth / lastScreenHeight >= canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y
            ? 1.0f
            : 0.0f;

        canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
    }

    private void Reset()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }
}
