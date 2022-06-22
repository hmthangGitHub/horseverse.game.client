using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : InjectedBState
{
    private UILoadingPresenter uiLoadingPresenter;
    public UILoadingPresenter UiLoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();

    private UIHeaderPresenter uiHeaderPresenter;
    public UIHeaderPresenter UiHeaderPresenter => uiHeaderPresenter ??= this.Container.Inject<UIHeaderPresenter>();

    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    public UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= this.Container.Inject<UIHorse3DViewPresenter>();

    private UIMainMenuPresenter uiMainMenuPresenter = new UIMainMenuPresenter();

    public override void Enter()
    {
        base.Enter();
        SubcribeEvents();
        UiLoadingPresenter.HideLoading();
        UIHorse3DViewPresenter.ShowHorse3DViewAsync().Forget();
        UiHeaderPresenter.ShowHeaderAsync().Forget();
        uiMainMenuPresenter.ShowMainMenuAsync().Forget();
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
        uiMainMenuPresenter.Dispose();
    }
}
