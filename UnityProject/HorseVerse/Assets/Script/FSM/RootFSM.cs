using Assets.RobustFSM.Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootFSM : MonoFSM
{
    public override void AddStates()
    {
        AddState<HorsePickingState>();
        AddState<HorseRaceState>();
        SetInitialState<HorsePickingState>();
    }
}
