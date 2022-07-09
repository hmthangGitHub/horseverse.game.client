using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableHorseDetailState : InjectedBState
{
    private UIHorseDetailPresenter uiHorseStablePresenter = default;
    private UIHeaderPresenter uiHeaderPresenter = default;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter = default;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    public override void Enter()
    {
        base.Enter();
        Container.Bind(new LocalHorseDetailDomainService(Container));

        UIHeaderPresenter.ShowHeaderAsync(true).Forget();
        UIHeaderPresenter.OnBack += OnBack;

        UIHorse3DViewPresenter.ShowHorse3DViewAsync().Forget();

        uiHorseStablePresenter = new UIHorseDetailPresenter(Container);
        uiHorseStablePresenter.ShowUIHorseDetailAsync().Forget();
    }

    private void OnBack()
    {
        this.Machine.ChangeState<StableUIState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.OnBack -= OnBack;
        uiHorseStablePresenter.Dispose();
        uiHorseStablePresenter = default;
        Container.RemoveAndDisposeIfNeed<LocalHorseDetailDomainService>();
    }
}
