using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public enum UICanvasType
    {
        Default,
        Header,
        Loading
    }

    public static Canvas GetCanvas(UICanvasType canvasType)
    {
        return canvasType switch
        {
            UICanvasType.Default => DefaultCanvas,
            UICanvasType.Header => HeaderCanvas,
            UICanvasType.Loading => LoadingCanvas,
            _ => DefaultCanvas
        };
    }

    [SerializeField]
    private Canvas defaultCanvas;
    [SerializeField]
    private Canvas headerCanvas;
    [SerializeField]
    private Canvas loadingCanvas;

    public static Canvas DefaultCanvas { get; private set; }
    public static Canvas HeaderCanvas { get; private set; }
    public static Canvas LoadingCanvas { get; private set; }

    private void Awake()
    {
        DefaultCanvas = defaultCanvas;
        HeaderCanvas = headerCanvas;
        LoadingCanvas = loadingCanvas;
    }
}
