﻿using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;

public class BetModeUIState : InjectedBState
{
    private UIBetModePresenter uiBetModePresenter = default;
    private UIHeaderPresenter uiHeaderPresenter = default;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    public UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= this.Container.Inject<UIHorse3DViewPresenter>();
    private UILoadingPresenter uiLoadingPresenter = default;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ?? Container.Inject<UILoadingPresenter>();
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();

    public override void Enter()
    {
        base.Enter();
        
        Container.Bind(new LocalBetModeDomainService(Container));
        Container.Bind(new BetMatchRepository());
        OnEnterStateAsync().Forget();
    }

    private async UniTaskVoid OnEnterStateAsync()
    {
        uiHeaderPresenter = Container.Inject<UIHeaderPresenter>();
        uiHeaderPresenter.HideHeader();
        
        uiBetModePresenter = new UIBetModePresenter(Container);
        uiBetModePresenter.OnBack += OnBackToMainMenu;
        uiBetModePresenter.OnToRaceMode += OnToRaceMode;
        await UIHorse3DViewPresenter.HideHorse3DViewAsync();
        await uiBetModePresenter.ShowUIBetModeAsync();
    }

    private void OnToRaceMode()
    {
        UILoadingPresenter.ShowLoadingAsync().Forget();
        UIHorse3DViewPresenter.Dispose();
        UIHeaderPresenter.Dispose();
        this.Machine.ChangeState<HorseRaceState>();
    }

    private void OnBackToMainMenu()
    {
        this.GetMachine<BetModeState>().GetMachine<InitialState>().ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        Container.RemoveAndDisposeIfNeed<BetMatchRepository>();
        Container.RemoveAndDisposeIfNeed<LocalBetModeDomainService>();
        uiBetModePresenter.OnBack -= OnBackToMainMenu;
        uiBetModePresenter.OnToRaceMode -= OnToRaceMode;
        uiBetModePresenter?.Dispose();
        uiBetModePresenter = default;
    }
}