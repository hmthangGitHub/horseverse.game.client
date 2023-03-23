using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HorseRaceThirdPersonStats
{
    [field: SerializeField]
    public float HorizontalSpeed { get; set; }
    [field: SerializeField]
    public Vector2 ForwardSpeedRange { get; set; }
    [field: SerializeField]
    public float Acceleration { get; set; }
    [field: SerializeField]
    public float SprintTime { get; set; }
    [field: SerializeField]
    public float SprintNumber { get; set; }
    [field: SerializeField]
    public float SprintHealingTime { get; set; }
}

[Serializable]
public class HorseRaceThirdPersonData
{
    [field: SerializeField]
    public TargetGenerator TargetGenerator { get; set; }
    [field: SerializeField]
    public int InitialLane { get; set; }
    [field: SerializeField]
    public bool IsPlayer { get; set; }
    [field: SerializeField]
    public Vector3[] PredefineWayPoints { get; set; }
    [field: SerializeField]
    public HorseRaceThirdPersonStats HorseRaceThirdPersonStats { get; set; }
    public IPredefinePath PredefinePath => TargetGenerator.PredefinePath;
}
