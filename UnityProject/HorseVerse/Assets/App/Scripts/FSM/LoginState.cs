using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RobustFSM.Interfaces;
using UnityEngine;

public class LoginState : InjectedBState
{
    private LoginStatePresenter loginPresenter;
    public override void Enter()
    {
        base.Enter();
        OnStateEnterAsync().Forget();
    }

    private async UniTask OnStateEnterAsync()
    {
        loginPresenter = new LoginStatePresenter(Container);
        await loginPresenter.ConnectAndLoginAsync();
        this.Machine.ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        loginPresenter.Dispose();
    }
}