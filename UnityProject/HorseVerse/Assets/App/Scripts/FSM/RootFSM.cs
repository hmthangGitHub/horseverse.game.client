using Assets.RobustFSM.Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootFSM : MonoFSMContainer
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<InitialState>();
        SetInitialState<InitialState>();
    }
}
