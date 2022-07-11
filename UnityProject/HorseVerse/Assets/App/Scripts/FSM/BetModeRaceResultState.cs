using Cysharp.Threading.Tasks;

public class BetModeRaceResultState : InjectedBState
{
    private BetModeRaceResultPresenter presenter;
    private UIBackGroundPresenter uiBackGroundPresenter => Container.Inject<UIBackGroundPresenter>();
    public override void Enter()
    {
        base.Enter();
        presenter = new BetModeRaceResultPresenter(Container);
        ShowResultAndRewardAsync().Forget();
    }

    private async UniTaskVoid ShowResultAndRewardAsync()
    {
        await uiBackGroundPresenter.ShowBackGroundAsync();
        await presenter.ShowResultAsynnc();
        this.GetSuperMachine<RootFSM>().ToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        presenter?.Dispose();
    }
}