using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

public class BetModeInitialState : InjectedBState
{
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private IBetModeDomainService betModeDomainService;
    private IReadOnlyBetMatchRepository betMatchRepository;
    
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= this.Container.Inject<UIHorse3DViewPresenter>();
    private IBetModeDomainService BetModeDomainService => betModeDomainService ??= Container.Inject<IBetModeDomainService>();
    private IReadOnlyBetMatchRepository BetMatchRepository => betMatchRepository ??= Container.Inject<IReadOnlyBetMatchRepository>();
#if ENABLE_DEBUG_MODULE
    private BetModeUIDebugMenuPresenter betModeUIDebugMenuPresenter;
    private BetModeUIDebugMenuPresenter BetModeUIDebugMenuPresenter => betModeUIDebugMenuPresenter ??= Container.Inject<BetModeUIDebugMenuPresenter>();
#endif
    
    public override void Enter()
    {
        base.Enter();
        OnEnterAsync().Forget();
    }

    private async UniTaskVoid OnEnterAsync()
    {
        await UIHorse3DViewPresenter.HideHorse3DViewAsync();
        await BetModeDomainService.RequestBetData();
#if ENABLE_DEBUG_MODULE
        BetModeUIDebugMenuPresenter.UpdateMatchId();
#endif
        if (BetMatchRepository.Current.MatchStatus == MatchStatus.Acting)
        {
            this.Machine.ChangeState<BetModeInProgressState>();
        }
        else
        {
            this.Machine.ChangeState<BetModeUIState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
        betMatchRepository = default;
        betModeDomainService = default;
        uiHorse3DViewPresenter = default;
#if ENABLE_DEBUG_MODULE
        betModeUIDebugMenuPresenter = default;
#endif
    }
}