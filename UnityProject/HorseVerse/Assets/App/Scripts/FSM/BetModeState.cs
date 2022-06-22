using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetModeState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<BetModeUIState>();
        AddState<HorseRaceState>();
        SetInitialState<BetModeUIState>();
    }
}
