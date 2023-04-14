using Cysharp.Threading.Tasks;

public class TrainingUIState : InjectedBState
{
    private UIHorseTrainingPresenter presenter;
    private UIHeaderPresenter UIHeaderPresenter => Container.Inject<UIHeaderPresenter>();

    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    public override void Enter()
    {
        base.Enter();
        presenter = new UIHorseTrainingPresenter(Container);

        UIHeaderPresenter.OnBack += OnBack;
        UIHeaderPresenter.ShowHeaderAsync(true, "ADVENTURE").Forget();
        presenter.ToTrainingActionState += ToTrainingActionState;
        presenter.ShowUIHorseTrainingAsync().Forget();
    }

    private void ToTrainingActionState()
    {
        this.Machine.ChangeState<TrainingActionState>();
    }

    private void OnBack()
    {
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        Release();
    }

    void Release()
    {
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        presenter.Dispose();
        presenter = null;
    }
}
