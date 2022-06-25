using Assets.RobustFSM.Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootFSM : MonoFSMContainer
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<StartUpState>();
        SetInitialState<StartUpState>();
    }

    private void OnApplicationQuit()
    {
        CurrentState.Exit();
    }
}
