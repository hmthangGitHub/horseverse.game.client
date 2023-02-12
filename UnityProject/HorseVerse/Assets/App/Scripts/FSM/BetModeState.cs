using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BetModeState : InjectedBHState
{
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void AddStates()
    {
        base.AddStates();
        AddState<BetModeUIState>();
        AddState<BetModeInProgressState>();
        AddState<HorseRaceState>();
        AddState<EmptyState>();
        AddState<BetModeInitialState>();
        SetInitialState<BetModeInitialState>();
    }
    

    public override void Enter()
    {
        base.Enter();
        Container.Bind(new BetModeDomainService(Container));
        Container.Bind(new BetMatchRepository(Container));
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
        Container.RemoveAndDisposeIfNeed<BetModeDomainService>();
        HorseRaceContext.Reset();
    }
}