using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using RobustFSM.Interfaces;
using UnityEngine;

public class MainMenuState : InjectedBState
{
    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UiLoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UiHeaderPresenter => uiHeaderPresenter ??= this.Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= this.Container.Inject<UIHorse3DViewPresenter>();
    private UIMainMenuPresenter uiMainMenuPresenter;
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private StartUpStatePresenter startUpStatePresenter;
    private StartUpStatePresenter StartUpStatePresenter => startUpStatePresenter ??= Container.Inject<StartUpStatePresenter>();
    private CancellationTokenSource cts;
    
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();


    public override void Enter()
    {
        base.Enter();
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        ShowBackGrounAsync().Forget();
        UIHorse3DViewPresenter.ShowHorse3DViewAsync().Forget();
        UiHeaderPresenter.ShowHeaderAsync(false).Forget();
        uiMainMenuPresenter ??= new UIMainMenuPresenter(this.Container);
        SubcribeEvents();
        uiMainMenuPresenter.ShowMainMenuAsync().Forget();
        SoundController.PlayMusicBase();
    }

    private async UniTask ShowBackGrounAsync()
    {
        await UIBackGroundPresenter.ShowBackGroundAsync().AttachExternalCancellation(cts.Token);
        UiLoadingPresenter.HideLoading();
    }

    private void SubcribeEvents()
    {
        uiMainMenuPresenter.OnBetModeBtn+= ToBetModeState;
        uiMainMenuPresenter.OnBreedingBtn+= ToBreedingState;
        uiMainMenuPresenter.OnInventoryBtn+= ToInventoryState;
        uiMainMenuPresenter.OnLibraryBtn+= ToLibraryState;
        uiMainMenuPresenter.OnPlayBtn+= ToQuickRaceState;
        uiMainMenuPresenter.OnStableBtn += ToStableState;
        uiMainMenuPresenter.OnTraningBtn += ToTrainingState;
    }

    private void UnSubcribeEvents()
    {
        uiMainMenuPresenter.OnBetModeBtn -= ToBetModeState;
        uiMainMenuPresenter.OnBreedingBtn -= ToBreedingState;
        uiMainMenuPresenter.OnInventoryBtn -= ToInventoryState;
        uiMainMenuPresenter.OnLibraryBtn -= ToLibraryState;
        uiMainMenuPresenter.OnPlayBtn -= ToQuickRaceState;
        uiMainMenuPresenter.OnStableBtn -= ToStableState;
        uiMainMenuPresenter.OnTraningBtn -= ToTrainingState;
    }

    private void ToStableState()
    {
        this.Machine.ChangeState<StableState>();
    }

    private void ToLibraryState()
    {
        this.Machine.ChangeState<BetModeState>();
    }

    private void ToInventoryState()
    {
        this.Machine.ChangeState<BetModeState>();
    }

    private void ToBreedingState()
    {
        this.Machine.ChangeState<BetModeState>();
    }

    private void ToBetModeState()
    {
        this.Machine.ChangeState<BetModeState>();
    }

    private void ToQuickRaceState()
    {
        this.Machine.ChangeState<QuickRaceState>();
    }

    private void ToTrainingState()
    {
        this.Machine.ChangeState<TrainingState>();
    }

    public override void Exit()
    {
        base.Exit();
        UnSubcribeEvents();
        UiHeaderPresenter.HideHeader();
        uiMainMenuPresenter.Dispose();
        uiMainMenuPresenter = default;
        uiLoadingPresenter = default;
        uiHeaderPresenter = default;
        uiHorse3DViewPresenter = default;
        uiBackGroundPresenter = default;
        socketClient = default;
        cts.SafeCancelAndDispose();
        cts = default;
    }
}
