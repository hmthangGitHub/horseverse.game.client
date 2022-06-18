using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableUIState : InjectedBState
{
    private UIHorseStablePresenter uiHorseStablePresenter = default;
    private UIHeaderPresenter uiHeaderPresenter = default;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter = default;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();

    public override void Enter()
    {
        base.Enter();
        UIHeaderPresenter.HideHeader();
        UIHorse3DViewPresenter.HideHorse3DView();
        uiHorseStablePresenter ??= new UIHorseStablePresenter(Container);
        uiHorseStablePresenter.OnViewHorseDetail += OnViewHorseDetail;
        uiHorseStablePresenter.OnBack += OnBack;
        uiHorseStablePresenter.ShowUIHorseStableAsync().Forget();
    }

    private void OnBack()
    {
        this.SuperMachine.GetInitialState<InitialState>().ChangeState<MainMenuState>();
    }

    private void OnViewHorseDetail()
    {
        this.Machine.ChangeState<StableHorseDetailState>();
    }

    public override void Exit()
    {
        base.Exit();
        uiHorseStablePresenter.OnViewHorseDetail -= OnViewHorseDetail;
        uiHorseStablePresenter.OnBack -= OnBack;
        uiHorseStablePresenter.Dispose();
        uiHorseStablePresenter = default;
    }
}
