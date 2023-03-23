using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseRaceActionState : InjectedBHState
{
    private bool isGameStart = false;
    private HorseRacePresenter horseRacePresenter;
    private UILoadingPresenter uiLoadingPresenter;
    private UIHeaderPresenter uiHeaderPresenter;

    private UILoadingPresenter UiLoadingPresenter => uiLoadingPresenter ??= Container.Inject<UILoadingPresenter>();
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private IPingDomainService pingDomainService;
    private IPingDomainService PingDomainService => pingDomainService ??= Container.Inject<PingDomainService>();

    public override void Enter()
    {
        base.Enter();
        OnEnterAsync().Forget();
        PingDomainService.StopPingService();
    }

    private async UniTask OnEnterAsync()
    {
        await UiLoadingPresenter.ShowLoadingAsync();
        UIHeaderPresenter.ReleaseHeaderUI();
        
        horseRacePresenter = new HorseRacePresenter(Container);
        horseRacePresenter.OnToBetModeResultState += ToBetModeResultState;
        horseRacePresenter.OnToQuickRaceModeResultState += ToRacingResultState;
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
        AddState<RacingResultState>();
        SetInitialState<EmptyState>();
    }

    private void ToBetModeResultState()
    {
        horseRacePresenter.Dispose();
        this.ChangeState<BetModeRaceResultState>();
    }

    private void ToRacingResultState()
    {
        horseRacePresenter.Dispose();
        this.ChangeState<RacingResultState>();
    }

    private async UniTask StartRaceAsync()
    {
        await horseRacePresenter.PlayIntro();
        isGameStart = true;
        horseRacePresenter.StartGame();
    }

    public override void PhysicsExecute()
    {
        base.PhysicsExecute();
        horseRacePresenter?.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
        PingDomainService.StartPingService().Forget();
        isGameStart = false;
        uiLoadingPresenter = default;
        horseRacePresenter.OnToBetModeResultState -= ToBetModeResultState;
        horseRacePresenter.OnToQuickRaceModeResultState -= ToRacingResultState;
        DisposeUtility.SafeDispose(ref horseRacePresenter);
    }
}
