﻿using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using System.Threading;
using UnityEngine;

public class LoadingState : InjectedBState
{
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();
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
        await UniTask.Delay(1000).AttachExternalCancellation(cts.Token);
        // await SocketClient.Connect("tcp.prod.game.horsesoflegends.com", 8770);
        //await SocketClient.Connect("127.0.0.1", 8080);
        this.Machine.ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        cts.SafeCancelAndDispose();
        cts = default;
    }
}