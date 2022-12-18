using PathCreation;
using System;
using UnityEngine;

public class HorseInGameData
{
    public bool IsPlayer { get; set; }
    public float CurrentOffset { get; set; }
    public int TopInRaceMatch { get; set; }
    public int InitialLane { get; set; }
    public TargetGenerator TargetGenerator { get; set; }
    public ((Vector3 target, float time)[] targets, int finishIndex) PredefineTargets { get; set; }
    public Action OnFinishTrack { get; set; }
    public GameObject MainCamera { get; set; }
    public float Delay { get; set; }
}