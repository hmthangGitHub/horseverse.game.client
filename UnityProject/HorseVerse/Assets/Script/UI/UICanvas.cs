using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public Canvas defaultCanvas;
    public static Canvas DefaultCanvas { get; private set; }

    private void Awake()
    {
        DefaultCanvas = defaultCanvas;
    }
}
