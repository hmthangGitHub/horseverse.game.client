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
    public UILoadingPresenter UiLoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();
    private UIHeaderPresenter uiHeaderPresenter;
    public UIHeaderPresenter UiHeaderPresenter => uiHeaderPresenter ??= this.Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    public UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= this.Container.Inject<UIHorse3DViewPresenter>();
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
        UiHeaderPresenter.OnLogOut += OnLogOut;
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
    
    
    private void OnLogOut()
    {
        OnLogOutAsync().Forget();
    }

    private async UniTask OnLogOutAsync()
    {
        await uiHorse3DViewPresenter.HideHorse3DViewAsync();
        uiHorse3DViewPresenter.Dispose();
        await SocketClient.Close();
#if MULTI_ACCOUNT
        var indexToken = PlayerPrefs.GetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, "");
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_STORAGE + indexToken);
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_CURRENT_KEY_INDEX);
#else
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_STORAGE);
#endif
        AudioManager.Instance?.StopMusic();
        StartUpStatePresenter.Reboot();
        startUpStatePresenter = default;
    }

    public override void Exit()
    {
        base.Exit();
        UnSubcribeEvents();
        UiHeaderPresenter.HideHeader();
        UiHeaderPresenter.OnLogOut -= OnLogOut;
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
