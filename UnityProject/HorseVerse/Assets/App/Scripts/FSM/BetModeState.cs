using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;

public class BetModeState : InjectedBState
{
    private IBetRateRepository betRateRepository = default;
    private UIBetModePresenter uiBetModePresenter = default;
    private UIHeaderPresenter uiHeaderPresenter = default;

    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    public UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= this.Container.Inject<UIHorse3DViewPresenter>();

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

        UIHorse3DViewPresenter.HideHorse3DView();
        await uiBetModePresenter.ShowUIBetModeAsync();
    }

    private void OnBackToMainMenu()
    {
        this.Machine.ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();

        Container.RemoveAndDisposeIfNeed<BetMatchRepository>();
        Container.RemoveAndDisposeIfNeed<LocalBetModeDomainService>();
        uiBetModePresenter.OnBack -= OnBackToMainMenu;
        uiBetModePresenter?.Dispose();
        uiBetModePresenter = default;
    }
}