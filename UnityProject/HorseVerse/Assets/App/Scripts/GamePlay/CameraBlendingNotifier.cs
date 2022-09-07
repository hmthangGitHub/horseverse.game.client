using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

[RequireComponent(typeof(CinemachineBrain))]
 
public class CameraBlendingNotifier : MonoBehaviour
{
    public UnityEvent OnStartBlending;
    public UnityEvent OnEndBlending;
    
    public delegate void CameraBlendStarted();
    public static event CameraBlendStarted onCameraBlendStarted;
 
    public delegate void CameraBlendFinished();
    public static event CameraBlendFinished onCameraBlendFinished;
 
    private CinemachineBrain cineMachineBrain;
 
    private bool wasBlendingLastFrame;
 
    void Awake()
    {
        cineMachineBrain = GetComponent<CinemachineBrain>();
        
    }
    void Start()
    {
        wasBlendingLastFrame = false;
    }
 
    void Update()
    {
        if (cineMachineBrain.IsBlending)
        {
            if (!wasBlendingLastFrame)
            {
                OnStartBlending.Invoke();
                if (onCameraBlendStarted != null)
                {
                    OnStartBlending.Invoke();
                    onCameraBlendStarted();
                }
            }
 
            wasBlendingLastFrame = true;
        }
        else
        {
            if (wasBlendingLastFrame)
            {
                OnEndBlending.Invoke();
                if (onCameraBlendFinished != null)
                {
                    onCameraBlendFinished();
                }
                wasBlendingLastFrame = false;
            }
        }
    }
}