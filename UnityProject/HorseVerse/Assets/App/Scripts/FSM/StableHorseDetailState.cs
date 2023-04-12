using Cysharp.Threading.Tasks;

public class StableHorseDetailState : InjectedBState
{
    private UIHorseDetailPresenter uiHorseStablePresenter = default;
    private UIHeaderPresenter uiHeaderPresenter = default;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter = default;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    public override void Enter()
    {
        base.Enter();
        Container.Bind(new LocalHorseDetailDomainService(Container));

        UIHeaderPresenter.ShowHeaderAsync(true, "STABLE").Forget();
        UIHeaderPresenter.OnBack += OnBack;

        UIHorse3DViewPresenter.SetRotateEnable(true);
        UIHorse3DViewPresenter.ChangeCameraType(MainMenuCameraType.CameraType.StableDetail);
        uiHorseStablePresenter = new UIHorseDetailPresenter(Container);
        uiHorseStablePresenter.ShowUIHorseDetailAsync().Forget();
    }

    private void OnBack()
    {
        OnBackAsync().Forget();
    }

    private async UniTask OnBackAsync()
    {
        await uiHorseStablePresenter.OutAsync();
        this.Machine.ChangeState<StableUIState>();
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
        uiHorseStablePresenter.Dispose();
        uiHorseStablePresenter = default;
        Container.RemoveAndDisposeIfNeed<LocalHorseDetailDomainService>();
    }
}
