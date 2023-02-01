using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceMatchDataContext
{
    public RaceMode RaceMode { get; set; }

    public TraditionalRoomMasteryType TraditionalRoomMasteryType { get; set; }
}

public enum RaceMode
{
    None,
    Traditional,
    StableVsStable,
    Rank,
    Tournament
}

public enum TraditionalRoomMasteryType
{
    None,
    Novice,
    Basic,
    Advance,
    Expert,
    Master
}
