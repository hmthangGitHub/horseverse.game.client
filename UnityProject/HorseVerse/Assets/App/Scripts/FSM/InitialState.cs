using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialState : InjectedBHState, IDisposable
{
    public async override void Enter()
    {
        this.Container.Bind(new BetRateRepository());
        this.Container.Bind(new UserDataRepository());
        this.Container.Bind(new UILoadingPresenter());
        this.Container.Bind(new UIHeaderPresenter(Container));
        this.Container.Bind(new UIHorse3DViewPresenter(Container));
        this.Container.Bind(await MasterLoader.LoadAsync<MasterHorseContainer>());
        base.Enter();
    }

    public override void AddStates()
    {
        base.AddStates();

        AddState<LoadingState>();

        AddState<HorsePickingState>();
        AddState<HorseRaceState>();
        AddState<BetModeState>();
        AddState<MainMenuState>();

        SetInitialState<LoadingState>();
    }

    public override void Exit()
    {
        base.Exit();
        Dispose();
    }

    public void Dispose()
    {
        this.Container.RemoveAndDisposeIfNeed<BetRateRepository>();
        this.Container.RemoveAndDisposeIfNeed<UserDataRepository>();
        this.Container.RemoveAndDisposeIfNeed<UILoadingPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHeaderPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHorse3DViewPresenter>();
        this.Container.RemoveAndDisposeIfNeed<MasterHorseContainer>();
    }
}