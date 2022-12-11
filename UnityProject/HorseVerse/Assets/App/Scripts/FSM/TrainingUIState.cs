using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUIState : InjectedBState
{
    private UIHorseTrainingPresenter presenter;
    private UIHeaderPresenter UIHeaderPresenter => Container.Inject<UIHeaderPresenter>();
    public override void Enter()
    {
        base.Enter();
        presenter = new UIHorseTrainingPresenter(Container);

        UIHeaderPresenter.OnBack += OnBack;
        UIHeaderPresenter.ShowHeaderAsync(true, "ADVENTURE").Forget();
        presenter.ToTrainingActionState += ToTrainingActionState;
        presenter.ShowUIHorseTrainingAsync().Forget();
    }

    private void ToTrainingActionState()
    {
        this.Machine.ChangeState<TrainingActionState>();
    }

    private void OnBack()
    {
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        presenter.Dispose();
        presenter = null;
    }
}
