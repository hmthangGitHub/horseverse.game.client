using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceMatchDataContext
{
    public RaceMode RaceMode { get; set; }

    public RacingRoomType RacingRoomType { get; set; }
}

public enum RaceMode
{
    None,
    Traditional,
    StableVsStable,
    Rank,
    Tournament
}