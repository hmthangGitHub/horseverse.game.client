using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using UnityEngine;

public class LoadingState : InjectedBState
{
    public override void Enter()
    {
        base.Enter();
        ShowLoadingThenChangeState().Forget();
    }

    private async UniTaskVoid ShowLoadingThenChangeState()
    {
        var uiLoadingPresenter = this.Container.Inject<UILoadingPresenter>();
        uiLoadingPresenter.ShowLoadingAsync().Forget();
        await UniTask.Delay(1000);
        this.Machine.ChangeState<HorsePickingState>();
    }
}