using System;
using UnityEngine;

public class PlatformBase : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public Action OnFinishPlatform = ActionUtility.EmptyAction.Instance;
    public PlatformTrigger platformTrigger = default;
    
    [SerializeField]
    protected Transform blockContainer;
    [SerializeField]
    protected Transform sceneryContainer;
    [SerializeField]
    protected Transform coinContainer;

    public bool IsReady { get; set; } = true;

    public virtual void Clear()
    {
    }
}