using System;
using UnityEngine;

public class PlatformBase : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public Action OnFinishPlatform = ActionUtility.EmptyAction.Instance;
    
    [SerializeField]
    protected Transform blockContainer;

    public bool IsReady { get; set; } = true;
}