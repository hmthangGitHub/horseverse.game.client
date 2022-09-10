using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseRaceState : InjectedBHState
{
    private bool isGameStart = false;
    private HorseRacePresenter horseRacePresenter;
    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UiLoadingPresenter => uiLoadingPresenter ??= Container.Inject<UILoadingPresenter>();
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();

    public override async void Enter()
    {
        base.Enter();
        horseRacePresenter = new HorseRacePresenter(Container);
        horseRacePresenter.OnToBetModeResultState += ToBetModeResultState;
        horseRacePresenter.OnToQuickRaceModeResultState += ToQuickRaceResultState;
        UIBackGroundPresenter.ReleaseBackGround();

        await horseRacePresenter.LoadAssetAsync();
        UiLoadingPresenter.HideLoading();
        await StartRaceAsync();
    }

    public override void AddStates()
    {
        base.AddStates();
        AddState<EmptyState>();
        AddState<BetModeRaceResultState>();
        AddState<QuickRaceResultState>();
        SetInitialState<EmptyState>();
    }

    private void ToBetModeResultState()
    {
        this.ChangeState<BetModeRaceResultState>();
    }

    private void ToQuickRaceResultState()
    {
        this.ChangeState<QuickRaceResultState>();
    }

    private async UniTask StartRaceAsync()
    {
        await horseRacePresenter.PlayIntro();
        isGameStart = true;
        horseRacePresenter.StartGame();
    }

    private void ToMainState()
    {
        ToMainStateAsync().Forget();
    }

    private async UniTaskVoid ToMainStateAsync()
    {
        await this.Container.Inject<UILoadingPresenter>().ShowLoadingAsync();
        await UniTask.Delay(1000);
        this.SuperMachine.GetState<StartUpState>().GetState<InitialState>().ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        isGameStart = false;

        uiLoadingPresenter = default;
        horseRacePresenter.OnToBetModeResultState -= ToBetModeResultState;
        horseRacePresenter.OnToQuickRaceModeResultState -= ToQuickRaceResultState;
        horseRacePresenter.Dispose();
        horseRacePresenter = default;
        Container.RemoveAndDisposeIfNeed<RaceMatchData>();
    }
}
