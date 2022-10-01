using Assets.RobustFSM.Interfaces;
using Assets.RobustFSM.Mono;
using RobustFSM.Base;
using RobustFSM.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public void ChangeToChildStateRecursive<T>() where T : IState
    {
        var states = this.States;
        var state = States.FirstOrDefault(x => x.Key == typeof(T)).Value;
        var currentState = this.CurrentState;
        while (state == default)
        {
            if(currentState is IHState hState)
            {
                currentState = hState.CurrentState;
                state = hState.States.FirstOrDefault(x => x.Key == typeof(T)).Value;
            }
            else
            {
                throw new System.Exception($"No child state with type {typeof(T)} under active control");
            }
        }
        state.Machine.ChangeState<T>();
    }    
}
