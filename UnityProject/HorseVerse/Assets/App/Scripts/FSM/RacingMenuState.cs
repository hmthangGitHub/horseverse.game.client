using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RacingMenuState : InjectedBHState
{
    private UIRacePresenter uiRacePresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private HorseRaceContext horseRaceContext;
    
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();

    public override void AddStates()
    {
        base.AddStates();
        AddState<EmptyState>();
        AddState<RacingMatchFindingState>();
        SetInitialState<EmptyState>();
    }

    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    private async UniTask OnEnterStateAsync()
    {
        uiRacePresenter = new UIRacePresenter(this.Container);
        UIHeaderPresenter.ShowHeaderAsync(true, 
            HorseRaceContext.RaceMatchDataContext.RacingRoomType.ToString()).Forget();
        UIHeaderPresenter.OnBack += OnBack;
        uiRacePresenter.OnFindMatch += OnFindMatch;
        await uiRacePresenter.ShowUIQuickRaceAsync();
    }

    private void OnFindMatch()
    {
        ChangeState<RacingMatchFindingState>();
    }
    
    private void OnBack()
    {
        HorseRaceContext.RaceMatchDataContext.RacingRoomType = RacingRoomType.None;
        this.Machine.ChangeState<RaceModeChoosingState>();
    }

    public override void Exit()
    {
        base.Exit();
        Release();
    }

    void Release()
    {
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        uiRacePresenter.Dispose();
        
        uiRacePresenter = default;
        uiHeaderPresenter = default;
        horseRaceContext = default;
    }

}