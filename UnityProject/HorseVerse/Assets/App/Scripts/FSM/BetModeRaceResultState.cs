using Cysharp.Threading.Tasks;

public class BetModeRaceResultState : InjectedBState
{
    private BetModeRaceResultPresenter presenter;

    public override void Enter()
    {
        base.Enter();
        presenter = new BetModeRaceResultPresenter(Container);
        ShowResultAndRewardAsync().Forget();
    }

    private async UniTaskVoid ShowResultAndRewardAsync()
    {
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