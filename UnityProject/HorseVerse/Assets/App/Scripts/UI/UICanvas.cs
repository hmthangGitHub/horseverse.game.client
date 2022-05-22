using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public enum UICanvasType
    {
        Default,
        LoadingCanvas
    }

    public static Canvas GetCanvas(UICanvasType canvasType)
    {
        return canvasType switch
        {
            UICanvasType.Default => DefaultCanvas,
            UICanvasType.LoadingCanvas => LoadingCanvas,
            _ => DefaultCanvas
        };
    }

    [SerializeField]
    private Canvas defaultCanvas;
    [SerializeField]
    private Canvas loadingCanvas;

    public static Canvas DefaultCanvas { get; private set; }
    public static Canvas LoadingCanvas { get; private set; }

    private void Awake()
    {
        DefaultCanvas = defaultCanvas;
        LoadingCanvas = loadingCanvas;
    }
}
