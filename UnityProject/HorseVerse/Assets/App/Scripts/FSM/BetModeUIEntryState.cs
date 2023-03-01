using Cysharp.Threading.Tasks;

public class BetModeUIEntryState : InjectedBState
{
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();

    public override void Enter()
    {
        base.Enter();
        UIHeaderPresenter.OnBack += OnBackBtn;
        UIHeaderPresenter.ShowHeaderAsync(true, "ARENA").Forget();
    }
    
    private void OnBackBtn()
    {
        OnBackToMainMenu();
    }
    
    private void OnBackToMainMenu()
    {
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.OnBack -= OnBackBtn;
    }
}