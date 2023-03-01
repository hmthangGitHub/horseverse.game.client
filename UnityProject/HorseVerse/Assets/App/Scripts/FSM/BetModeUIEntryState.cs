using Cysharp.Threading.Tasks;

public class BetModeUIEntryState : InjectedBState
{
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();

    public override void Enter()
    {
        Enter();
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
        Exit();
        UIHeaderPresenter.OnBack -= OnBackBtn;
    }
}