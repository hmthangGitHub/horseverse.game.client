using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleRacingManager : MonoBehaviour
{
    
}

public struct RacingLapProgress
{
    public int CurrentLap;
    public int CurrentControlPoint;
    public float TotalProgress;
    public float CurrentControlPointProgress;
}

public struct RacingRank
{
    public int Value;
    public float LastLapTime;
}
