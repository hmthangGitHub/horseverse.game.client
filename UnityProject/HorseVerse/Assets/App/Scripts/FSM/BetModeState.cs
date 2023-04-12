public class BetModeState : InjectedBHState
{
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void AddStates()
    {
        base.AddStates();
        AddState<BetModeUIState>();
        AddState<BetModeInProgressState>();
        AddState<HorseRaceActionState>();
        AddState<EmptyState>();
        AddState<BetModeInitialState>();
        SetInitialState<BetModeInitialState>();
    }
    

    public override void Enter()
    {
        base.Enter();
        Container.Bind(new BetModeDomainService(Container));
        Container.Bind(new BetMatchRepository(Container));
        Container.Bind(new HorseRaceManagerFactory(Container));
        HorseRaceContext.GameMode = HorseGameMode.Bet;
#if ENABLE_DEBUG_MODULE
        Container.Bind(new BetModeUIDebugMenuPresenter(Container));
#endif
    }

    public override void Exit()
    {
        base.Exit();
#if ENABLE_DEBUG_MODULE
        Container.RemoveAndDisposeIfNeed<BetModeUIDebugMenuPresenter>();
#endif
        Container.RemoveAndDisposeIfNeed<BetMatchRepository>();
        Container.RemoveAndDisposeIfNeed<HorseRaceManagerFactory>();
        Container.RemoveAndDisposeIfNeed<BetModeDomainService>();
        HorseRaceContext.Reset();
    }
}