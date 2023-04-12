using Cysharp.Threading.Tasks;

public class LoginState : InjectedBState
{
    private LoginStatePresenter loginPresenter;
    private IPingDomainService pingDomainService;
    private IPingDomainService PingDomainService => pingDomainService ??= Container.Inject<IPingDomainService>();

    public override void Enter()
    {
        base.Enter();
        OnStateEnterAsync().Forget();
    }

    private async UniTask OnStateEnterAsync()
    {
        loginPresenter = new LoginStatePresenter(Container);
#if UNITY_WEBGL || WEB_SOCKET
        this.Machine.ChangeState<MainMenuState>();
        return;
#endif
        await loginPresenter.ConnectAndLoginAsync();
        PingDomainService.StartPingService().Forget();
        this.Machine.ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        loginPresenter.Dispose();
        loginPresenter = default;
    }
}