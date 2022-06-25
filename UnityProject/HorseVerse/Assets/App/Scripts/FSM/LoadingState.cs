using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using UnityEngine;

public class LoadingState : InjectedBState
{
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();
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
        //await SocketClient.Connect("tcp.prod.game.horsesoflegends.com", 8770);
        this.Machine.ChangeState<MainMenuState>();
    }
}