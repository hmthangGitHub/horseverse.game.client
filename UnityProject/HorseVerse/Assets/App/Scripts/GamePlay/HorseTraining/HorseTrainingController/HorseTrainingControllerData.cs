using System;
using UnityEngine;

[Serializable]
public class HorseTrainingControllerData
{
    [SerializeField]
    private float totalDistance;

    public float TotalDistance
    {
        get => totalDistance;
        set => totalDistance = value;
    }

    public float movingTime;
    public float Speed { get; set; }
    public TrainingPathBridge Bridge { get; set; }
    public float CurrentHeight { get; set; }
    public float CurrentOffset { get; set; }
    public int CurrentOffSetInt { get; set; }

    public Action OnBeginBridge { get; set; } = ActionUtility.EmptyAction.Instance;
    public Action OnFinishLandingBridge { get; set; } = ActionUtility.EmptyAction.Instance;
}