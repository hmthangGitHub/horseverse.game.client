using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRaceState : InjectedBHState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void AddStates()
    {
        base.AddStates();
        AddState<QuickRaceMenuState>();
        AddState<HorseRaceState>();
        SetInitialState<QuickRaceMenuState>();
    }
}
