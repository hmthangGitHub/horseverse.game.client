using Assets.RobustFSM.Interfaces;
using RobustFSM.Interfaces;
using UnityEngine;
using System.Linq;

public class RootFSM : MonoFSMContainer
{
    public override void AddStates()
    {
        Application.runInBackground = true;
#if !UNITY_EDITOR
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
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

    private void OnApplicationPause(bool pauseStatus)
    {
        var currentState = this.CurrentState;
        PassPauseEvent(pauseStatus, currentState);
        
        while (currentState is IHState hState)
        {
            currentState = hState.CurrentState;
            PassPauseEvent(pauseStatus, currentState);
        }
    }

    private static void PassPauseEvent(bool pauseStatus,
                                       IState currentState)
    {
        if (currentState is IOnApplicationPauseState pauseState)
        {
            pauseState.OnApplicationPause(pauseStatus);
        }
    }
}
