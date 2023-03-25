using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RobustFSM.Interfaces;
using UnityEngine;

public class RacingThirdPersonMatchFindingState : InjectedBState
{
    private RacingThirdPersonMatchFindingPresenter presenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UILoadingPresenter uiLoadingPresenter;
    private HorseRaceContext horseRaceContext;
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter UiHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();
    
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    private async UniTaskVoid OnEnterStateAsync()
    {
        presenter = new RacingThirdPersonMatchFindingPresenter(Container);
        await presenter.FindMatchAsync();
        OnFoundMatchAsync().Forget();
    }
    
    private async UniTaskVoid OnFoundMatchAsync ()
    {
        await UILoadingPresenter.ShowLoadingAsync();

        UiHorse3DViewPresenter.Dispose();
        UIHeaderPresenter.ReleaseHeaderUI();
        HorseRaceContext.MasterMapId = RacingState.MasterMapId;
        ((IState)Machine).Machine.ChangeState<HorseRaceActionState>();
    }

    public override void Exit()
    {
        base.Exit();
        DisposeUtility.SafeDispose(ref presenter);
        uiHorse3DViewPresenter = default;
        uiLoadingPresenter = default;
    }
}
