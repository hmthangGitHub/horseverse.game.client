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
    
    public override void Enter()
    {
        base.Enter();
        OnEnterAsync().Forget();
    }

    private async UniTaskVoid OnEnterAsync()
    {
        await UIHorse3DViewPresenter.HideHorse3DViewAsync();
        await BetModeDomainService.RequestBetData();
        
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
    }
}