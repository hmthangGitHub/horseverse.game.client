using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BetModeState : InjectedBHState
{
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= this.Container.Inject<UIHorse3DViewPresenter>();
    
    public override void AddStates()
    {
        base.AddStates();
        AddState<BetModeUIState>();
        AddState<BetModeInProgressState>();
        AddState<HorseRaceState>();
        AddState<EmptyState>();
        SetInitialState<EmptyState>();
    }

    public override void Enter()
    {
        base.Enter();
        OnEnterAsync().Forget();
    }

    private async UniTaskVoid OnEnterAsync()
    {
        await UIHorse3DViewPresenter.HideHorse3DViewAsync();
        if (UnityEngine.Random.Range(0, 5) == 1)
        {
            ChangeState<BetModeInProgressState>();
        }
        else
        {
            ChangeState<BetModeUIState>();
        }
    }
}
