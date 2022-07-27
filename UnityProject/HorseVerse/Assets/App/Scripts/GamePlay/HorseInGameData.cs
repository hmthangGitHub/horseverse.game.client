using PathCreation;
using System;
using UnityEngine;

public class HorseInGameData
{
    public bool IsPlayer { get; set; }
    public float CurrentOffset { get; set; }
    public int TopInRaceMatch { get; set; }
    public float NormalizePath { get; set; }
    public int InitialLane { get; set; }
    public PathCreator PathCreator { get; set; }
    public (Vector3 target, float time)[] PredefineTargets { get; set; }
    public Action OnFinishTrack { get; set; }
}