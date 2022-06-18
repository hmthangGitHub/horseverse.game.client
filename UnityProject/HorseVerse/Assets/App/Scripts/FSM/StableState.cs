using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<StableUIState>();
        AddState<StableHorseDetailState>();
        SetInitialState<StableUIState>();
    }
}
