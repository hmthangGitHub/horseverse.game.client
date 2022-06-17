using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuickRaceMenuState : InjectedBState
{
    private UIQuickRacePresenter uiQuickRacePresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();

    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIHorse3DViewPresenter UiHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();

    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();

    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    private async UniTask OnEnterStateAsync()
    {
        Container.Bind(new LocalQuickRaceDomainService(this.Container));
        uiQuickRacePresenter = new UIQuickRacePresenter(this.Container);
        uiQuickRacePresenter.OnBack += OnBack;
        uiQuickRacePresenter.OnFoundMatch += OnFoundMatch;
        await uiQuickRacePresenter.ShowUIQuickRaceAsync();
    }

    private void OnFoundMatch()
    {
        UILoadingPresenter.ShowLoadingAsync().Forget();

        UiHorse3DViewPresenter.Dispose();
        UIHeaderPresenter.Dispose();

        this.Machine.ChangeState<HorseRaceState>();
    }

    private void OnBack()
    {
        this.SuperMachine.GetState<InitialState>().ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        uiQuickRacePresenter.OnBack -= OnBack;
        uiQuickRacePresenter.Dispose();
        uiQuickRacePresenter = default;
        Container.RemoveAndDisposeIfNeed<LocalQuickRaceDomainService>();
    }
}
