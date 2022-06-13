using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialState : InjectedBHState, IDisposable
{
    private UILoadingPresenter uiloadingPresenter = new UILoadingPresenter();
    private UIHeaderPresenter uiHeaderPresenter = default;
    private IReadOnlyUserDataRepository userDataRepository = new UserDataRepository();

    public override void Enter()
    {
        this.Container.Bind<IReadOnlyUserDataRepository>(userDataRepository);
        this.Container.Bind<UILoadingPresenter>(uiloadingPresenter);
        this.Container.Bind<UIHeaderPresenter>(uiHeaderPresenter = new UIHeaderPresenter(Container));
        
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
        this.Container.RemoveAndDisposeIfNeed<IReadOnlyUserDataRepository>();
        this.Container.RemoveAndDisposeIfNeed<UILoadingPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHeaderPresenter>();
    }
}