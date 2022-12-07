using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRaceState : InjectedBHState
{
    public static long MasterMapId = 10001004;

    public override void AddStates()
    {
        base.AddStates();
        AddState<QuickRaceMenuState>();
        AddState<HorseRaceState>();
        AddState<RaceState>();
        SetInitialState<QuickRaceMenuState>();
    }
}
