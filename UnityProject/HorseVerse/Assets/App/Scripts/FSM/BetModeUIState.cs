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
        await uiBetModePresenter.ShowUIBetModeAsync();
    }

    private void OnToRaceMode()
    {
        UILoadingPresenter.ShowLoadingAsync().Forget();
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