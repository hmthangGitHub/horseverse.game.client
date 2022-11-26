using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BetModeState : InjectedBHState
{
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
    }

    public override void Exit()
    {
        base.Exit();
        Container.RemoveAndDisposeIfNeed<BetMatchRepository>();
        Container.RemoveAndDisposeIfNeed<BetModeDomainService>();
    }
}