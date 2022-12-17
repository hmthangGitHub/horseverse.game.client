using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;

public class BetModeUIState : InjectedBState
{
    private UIBetModePresenter uiBetModePresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= Container.Inject<UILoadingPresenter>();
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    
    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();

    }

    private async UniTaskVoid OnEnterStateAsync()
    {
        uiHeaderPresenter = Container.Inject<UIHeaderPresenter>();
        uiHeaderPresenter.HideHeader();
        
        uiBetModePresenter = new UIBetModePresenter(Container);
        uiBetModePresenter.OnBack += OnBackToMainMenu;
        uiBetModePresenter.OnToRaceMode += OnToRaceMode;
        uiBetModePresenter.OnTimeOut += OnTimeOut;
        await uiBetModePresenter.ShowUIBetModeAsync();
    }

    private void OnTimeOut()
    {
        ((RootFSM)SuperMachine).ChangeToChildStateRecursive<BetModeState>();
    }

    private void OnToRaceMode()
    {
        OnToRaceModeAsync().Forget();
    }
    
    private async UniTask OnToRaceModeAsync()
    {
        await UILoadingPresenter.ShowLoadingAsync();
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
        uiBetModePresenter.OnBack -= OnBackToMainMenu;
        uiBetModePresenter.OnToRaceMode -= OnToRaceMode;
        uiBetModePresenter.OnTimeOut -= OnBackToMainMenu;
        uiBetModePresenter?.Dispose();
        uiBetModePresenter = default;
    }
}