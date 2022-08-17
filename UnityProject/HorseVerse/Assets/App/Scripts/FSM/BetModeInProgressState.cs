public class BetModeInProgressState : InjectedBState
{
    private UIBetModeInProgressStatePresenter presenter; 
    
    
    public override void Enter()
    {
        base.Enter();
        presenter = new UIBetModeInProgressStatePresenter(Container);
        presenter.ShowInProgressAsync().Forget();
        presenter.OnBack += OnBack;
    }

    private void OnBack()
    {
        this.GetSuperMachine<RootFSM>().ToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        presenter.Dispose();
    }
}