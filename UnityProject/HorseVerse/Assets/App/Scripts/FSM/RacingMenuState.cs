using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RacingMenuState : InjectedBState
{
    private UIRacePresenter uiRacePresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UILoadingPresenter uiLoadingPresenter;
    private HorseRaceContext horseRaceContext;
    
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter UiHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();

    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    private async UniTask OnEnterStateAsync()
    {
        uiRacePresenter = new UIRacePresenter(this.Container);
        UIHeaderPresenter.ShowHeaderAsync(true, 
            HorseRaceContext.RaceMatchDataContext.TraditionalRoomMasteryType.ToString()).Forget();
        UIHeaderPresenter.OnBack += OnBack;
        uiRacePresenter.OnFoundMatch += OnFoundMatch;
        await uiRacePresenter.ShowUIQuickRaceAsync();
    }

    private void OnFoundMatch (RaceScriptData data)
    {
        OnFoundMatchAsync(data).Forget();
    }

    private async UniTaskVoid OnFoundMatchAsync (RaceScriptData data)
    {
        await UILoadingPresenter.ShowLoadingAsync();

        UiHorse3DViewPresenter.Dispose();
        UIHeaderPresenter.Dispose();
        Container.Bind(data);
        this.Machine.ChangeState<HorseRaceState>();
    }

    private void OnBack()
    {
        HorseRaceContext.RaceMatchDataContext.TraditionalRoomMasteryType = TraditionalRoomMasteryType.None;
        this.Machine.ChangeState<RaceModeChoosingState>();
    }

    public override void Exit()
    {
        base.Exit();
        Release();
    }

    void Release()
    {
        uiRacePresenter.OnFoundMatch -= OnFoundMatch;
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        uiRacePresenter.Dispose();
        
        uiRacePresenter = default;
        uiHeaderPresenter = default;
        uiHorse3DViewPresenter = default;
        uiLoadingPresenter = default;
        horseRaceContext = default;
    }

}
