using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCanvas : MonoBehaviour
{
    public enum UICanvasType
    {
        BackGround,
        Default,
        Header,
        Loading,
        Debug,
    }

    public static Canvas GetCanvas(UICanvasType canvasType)
    {
        return canvasType switch
        {
            UICanvasType.BackGround => BackgroundCanvas,
            UICanvasType.Default => DefaultCanvas,
            UICanvasType.Header => HeaderCanvas,
            UICanvasType.Loading => LoadingCanvas,
            UICanvasType.Debug => DebugCanvas,
            _ => DefaultCanvas
        };
    }

    [SerializeField]
    private Canvas backgroundCanvas;
    [SerializeField]
    private Canvas defaultCanvas;
    [SerializeField]
    private Canvas headerCanvas;
    [SerializeField]
    private Canvas loadingCanvas;
    [SerializeField]
    private Canvas debugUICanvas;

    public static Canvas BackgroundCanvas { get; private set; }
    public static Canvas DefaultCanvas { get; private set; }
    public static Canvas HeaderCanvas { get; private set; }
    public static Canvas LoadingCanvas { get; private set; }
    public static Canvas DebugCanvas { get; private set; }

    private void Awake()
    {
        DefaultCanvas = defaultCanvas;
        HeaderCanvas = headerCanvas;
        LoadingCanvas = loadingCanvas;
        BackgroundCanvas = backgroundCanvas;
        DebugCanvas = debugUICanvas;
    }
}
