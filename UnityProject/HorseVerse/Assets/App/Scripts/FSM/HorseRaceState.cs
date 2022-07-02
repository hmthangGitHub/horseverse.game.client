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

    public override async void Enter()
    {
        base.Enter();
        horseRacePresenter = new HorseRacePresenter(Container);
        horseRacePresenter.OnBackToMainState += ToMainState;
        await horseRacePresenter.LoadAssetAsync();
        isLoadedUI = true;

        uiLoadingPresenter = this.Container.Inject<UILoadingPresenter>();
        uiLoadingPresenter.HideLoading();

        await horseRacePresenter.PlayIntro();
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

    public override void Execute()
    {
        base.ManualExecute();
        if (isLoadedUI && !isGameStart && (Input.touchCount >= 1 || Input.anyKey))
        {
            isGameStart = true;
            horseRacePresenter.StartGame();
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
