using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingState : InjectedBState
{
    private UIHorseTrainingPresenter presenter;
    public override void Enter()
    {
        base.Enter();
        presenter = new UIHorseTrainingPresenter(Container);
        presenter.OnBack += OnBack;
        presenter.ShowUIHorseTraningAsync().Forget();
    }

    private void OnBack()
    {
        this.Machine.ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        presenter.OnBack -= OnBack;
        presenter.Dispose();
        presenter = null;
    }
}
