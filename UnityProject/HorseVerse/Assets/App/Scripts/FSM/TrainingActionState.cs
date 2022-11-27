using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TrainingActionState : InjectedBState
{
    private UILoadingPresenter uiLoadingPresenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIBackGroundPresenter uiBackGroundPresenter;
    private HorseTrainingPresenter horseTrainingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= Container.Inject<UILoadingPresenter>();
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();

    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }
        
    private async UniTask OnEnterStateAsync()
    {
        await HideHorseAndBackGround();
        horseTrainingPresenter = new HorseTrainingPresenter(Container);
        await horseTrainingPresenter.LoadAssetsAsync();
        UILoadingPresenter.HideLoading();
        
        var coinCollected = await horseTrainingPresenter.StartTrainingAsync();

        await ShowHorseAndBackground();
        this.Machine.ChangeState<TrainingUIState>();
        UILoadingPresenter.HideLoading();
    }

    private async UniTask ShowHorseAndBackground()
    {
        await UILoadingPresenter.ShowLoadingAsync();
        await UIHorse3DViewPresenter.ShowHorse3DViewAsync();
        await UIBackGroundPresenter.ShowBackGroundAsync();
    }

    private async UniTask HideHorseAndBackGround()
    {
        await UIHorse3DViewPresenter.HideHorse3DViewAsync();
        await UILoadingPresenter.ShowLoadingAsync();
        UIHorse3DViewPresenter.Dispose();
        UIBackGroundPresenter.Dispose();
    }

    public override void Exit()
    {
        base.Exit();
        OnEditStateAsync().Forget();
    }

    private async UniTask OnEditStateAsync()
    {
        Container.RemoveAndDisposeIfNeed<HorseTrainingDataContext>();
        uiLoadingPresenter = default;
        uiHorse3DViewPresenter = default;
        uiBackGroundPresenter = default;
        DisposeUtility.SafeDispose(ref horseTrainingPresenter);
        await UniTask.CompletedTask;
    }
}