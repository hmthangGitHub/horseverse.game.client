using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public enum UICanvasType
    {
        BackGround,
        Default,
        Header,
        Loading,
        Debug,
        Error,
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
            UICanvasType.Error => ErrorCanvas,
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
    [SerializeField]
    private Canvas errorUICanvas;

    private static Canvas BackgroundCanvas { get; set; }
    private static Canvas DefaultCanvas { get; set; }
    private static Canvas HeaderCanvas { get; set; }
    private static Canvas LoadingCanvas { get; set; }
    private static Canvas DebugCanvas { get; set; }
    private static Canvas ErrorCanvas { get; set; }

    private void Awake()
    {
        DefaultCanvas = defaultCanvas;
        HeaderCanvas = headerCanvas;
        LoadingCanvas = loadingCanvas;
        BackgroundCanvas = backgroundCanvas;
        DebugCanvas = debugUICanvas;
        ErrorCanvas = errorUICanvas;
    }
}
