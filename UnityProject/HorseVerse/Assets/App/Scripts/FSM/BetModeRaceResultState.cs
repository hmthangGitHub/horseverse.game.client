using Cysharp.Threading.Tasks;

public class BetModeRaceResultState : InjectedBState
{
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private BetModeRaceResultPresenter presenter;

    public override void Enter()
    {
        base.Enter();
        OnEnterAsync().Forget();
    }

    private async UniTaskVoid OnEnterAsync()
    {
        await UIBackGroundPresenter.ShowBackGroundAsync();
        presenter = new BetModeRaceResultPresenter(Container);
        await presenter.ShowResultAsync();
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        presenter?.Dispose();
        presenter = null;
    }
}