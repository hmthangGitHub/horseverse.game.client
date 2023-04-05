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
    public float PercentageSpeedBoostWhenSprint { get; set; }
    [field: SerializeField]
    public float PercentageSpeedBonusBoostWhenSprintContinuously { get; set; }
    [field: SerializeField]
    public Vector2 AccelerationRange { get; set; }
    [field: SerializeField]
    public float SprintTime { get; set; }
    [field: SerializeField]
    public int SprintChargeNumber { get; set; }
    [field: SerializeField]
    public float SprintHealingTime { get; set; }
    [field: SerializeField]
    public float SprintBonusTime { get; set; }
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
    [field: SerializeField]
    public Transform Camera { get; set; }
    [field: SerializeField]
    public string Name { get; set; }
}
