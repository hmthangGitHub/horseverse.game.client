using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TrainingActionState : InjectedBState
{
    private UILoadingPresenter uiLoadingPresenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIBackGroundPresenter uiBackGroundPresenter;
    private HorseTrainingPresenter presenter;
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

        presenter = new HorseTrainingPresenter(Container);
        await presenter.LoadAssetsAsync();
        UILoadingPresenter.HideLoading();
        var coinCollected = await presenter.StartTrainingAsync();
        
        await UILoadingPresenter.ShowLoadingAsync();
        await ShowHorseAndBackground();
        this.Machine.ChangeState<TrainingUIState>();
        UILoadingPresenter.HideLoading();
    }

    private async UniTask ShowHorseAndBackground()
    {
        await UniTask.Delay(500);
        UIHorse3DViewPresenter.ShowHorse3DViewAsync().Forget();
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
        Container.RemoveAndDisposeIfNeed<HorseTrainingDataContext>();
        uiLoadingPresenter = default;
        uiHorse3DViewPresenter = default;
        uiBackGroundPresenter = default;
        presenter.Dispose();
        presenter = default;
    }
}