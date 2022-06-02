using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialState : InjectedBHState, IDisposable
{
    private UILoadingPresenter uiloadingPresenter = new UILoadingPresenter();

    public override void Enter()
    {
        this.Container.Bind<UILoadingPresenter>(uiloadingPresenter);
        base.Enter();
    }

    public override void AddStates()
    {
        base.AddStates();

        AddState<LoadingState>();

        AddState<HorsePickingState>();
        AddState<HorseRaceState>();

        SetInitialState<LoadingState>();
    }

    public override void Exit()
    {
        base.Exit();
        Dispose();
    }

    public void Dispose()
    {
        this.Container.RemoveAndDisposeIfNeed<UILoadingPresenter>();
    }
}