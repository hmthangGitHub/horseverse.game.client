using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HorseRaceThirdPersonMasterData
{
    [field: SerializeField]
    public bool IsPlayer { get; set; }
    [field: SerializeField]
    public Vector3[] PredefineWayPoints { get; set; }
    [field: SerializeField]
    public float HorizontalSpeed { get; set; }
    [field: SerializeField]
    public Vector2 ForwardSpeedRange { get; set; }
    [field: SerializeField]
    public float Acceleration { get; set; }
    [field: SerializeField]
    public TargetGenerator TargetGenerator { get; set; }
    [field: SerializeField]
    public float SprintTime { get; set; }
    [field: SerializeField]
    public float SprintNumber { get; set; }
    [field: SerializeField]
    public float SprintHealingTime { get; set; }
    [field: SerializeField]
    public float InitialLane { get; set; }
    public PredefinePath PredefinePath => TargetGenerator.PredefinePath;
}
