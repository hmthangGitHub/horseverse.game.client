using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseRaceState : InjectedBState
{
    private bool isGameStart = false;
    private bool isLoadedUI = false;
    private HorseRacePresenter horseRacePresenter;
    private UILoadingPresenter uiLoadingPresenter;

    public UILoadingPresenter UiLoadingPresenter => uiLoadingPresenter ??= Container.Inject<UILoadingPresenter>();

    public override async void Enter()
    {
        base.Enter();
        horseRacePresenter = new HorseRacePresenter(Container);
        horseRacePresenter.OnBackToMainState += ToMainState;
        await horseRacePresenter.LoadAssetAsync();
        isLoadedUI = true;

        UiLoadingPresenter.HideLoading();
        await StartRaceAsync();
    }

    private async UniTask StartRaceAsync()
    {
        await horseRacePresenter.PlayIntro();
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

    public override void PhysicsExecute()
    {
        base.PhysicsExecute();
        if (isGameStart)
        {
            horseRacePresenter.UpdateRaceStatus();
        }
    }

    public override void Exit()
    {
        base.Exit();
        isGameStart = false;
        isLoadedUI = false;

        uiLoadingPresenter = default;
        horseRacePresenter.OnBackToMainState -= ToMainState;
        horseRacePresenter.Dispose();
        horseRacePresenter = default;
        Container.RemoveAndDisposeIfNeed<RaceMatchData>();
    }
}
