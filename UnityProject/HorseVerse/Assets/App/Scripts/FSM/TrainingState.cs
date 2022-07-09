using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingState : InjectedBState
{
    private UIHorseTrainingPresenter presenter;
    private UIHeaderPresenter UIHeaderPresenter => Container.Inject<UIHeaderPresenter>();
    public override void Enter()
    {
        base.Enter();
        presenter = new UIHorseTrainingPresenter(Container);

        UIHeaderPresenter.OnBack += OnBack;
        UIHeaderPresenter.ShowHeaderAsync(true).Forget();
        presenter.ShowUIHorseTraningAsync().Forget();
    }
    
    private void OnBack()
    {
        this.Machine.ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.OnBack -= OnBack;
        presenter.Dispose();
        presenter = null;
    }
}
