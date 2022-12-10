using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using System.Threading;
using io.hverse.game.protogen;
using UnityEngine;

public class LoadingState : InjectedBState
{
    private CancellationTokenSource cts;
    public override void Enter()
    {
        base.Enter();
        ShowLoadingThenChangeState().Forget();
    }

    private async UniTaskVoid ShowLoadingThenChangeState()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        var uiLoadingPresenter = this.Container.Inject<UILoadingPresenter>();
        uiLoadingPresenter.ShowLoadingAsync().Forget();

        var audioPresenter = this.Container.Inject<AudioPresenter>();
        audioPresenter.ShowAudioAsync().Forget();

        await UniTask.Delay(1000).AttachExternalCancellation(cts.Token);
        this.Machine.ChangeState<LoginState>();
    }

    public override void Exit()
    {
        base.Exit();
        cts.SafeCancelAndDispose();
        cts = default;
    }
}