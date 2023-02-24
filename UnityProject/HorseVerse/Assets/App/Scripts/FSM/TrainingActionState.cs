using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TrainingActionState : InjectedBState
{
    private UILoadingPresenter uiLoadingPresenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= Container.Inject<UILoadingPresenter>();
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private CancellationTokenSource cts;

    public override void Enter()
    {
        base.Enter();
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        OnEnterStateAsync().Forget();
    }
        
    private async UniTask OnEnterStateAsync()
    {
        await HideHorseAndBackGround();
        await StartTraining();
        await ShowHorseAndBackground();
        this.Machine.ChangeState<TrainingUIState>();
        UILoadingPresenter.HideLoading();
    }

    private async UniTask StartTraining()
    {
        while (true)
        {
            using var horseTrainingPresenter = new HorseTrainingPresenter(Container);
            await horseTrainingPresenter.LoadAssetsAsync().AttachExternalCancellation(cts.Token);
            UILoadingPresenter.HideLoading();
            if (!await horseTrainingPresenter.StartTrainingAsync().AttachExternalCancellation(cts.Token))
            {
                break;
            }
            else
            {
                await UILoadingPresenter.ShowLoadingAsync();    
            }
        }
    }

    private async UniTask ShowHorseAndBackground()
    {
        await UILoadingPresenter.ShowLoadingAsync();
        await UIHorse3DViewPresenter.ShowHorse3DViewAsync();
        await UIBackGroundPresenter.HideBackground();
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
        DisposeUtility.SafeDispose(ref cts);
        Container.RemoveAndDisposeIfNeed<HorseTrainingDataContext>();
        uiLoadingPresenter = default;
        uiHorse3DViewPresenter = default;
        uiBackGroundPresenter = default;
        await UniTask.CompletedTask;
    }
}