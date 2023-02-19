using RobustFSM.Base;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class HorsePickingState : InjectedBState
{
    private UIHorsePicker uiHorsePicker = default;
    private CancellationTokenSource cts = default;
    private int HorseId { get; set; }

    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UiLoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();

    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UiHeaderPresenter => uiHeaderPresenter ??= this.Container.Inject<UIHeaderPresenter>();

    public override async void Enter()
    {
        base.Enter();
        UiLoadingPresenter.HideLoading();
        UiHeaderPresenter.ShowHeaderAsync().Forget();
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiHorsePicker ??= await UILoader.Instantiate<UIHorsePicker>(token: cts.Token);
        uiHorsePicker.SetEntity(new UIHorsePicker.Entity()
        {
            horseLoader = new HorseLoader.Entity()
            {
                horse = HorseMasterDataContainer.HorseModelPaths[HorseId]
            },
            left = new ButtonComponent.Entity(() =>
            {
                HorseId = ((HorseId - 1 + HorseMasterDataContainer.HorseModelPaths.Count) % HorseMasterDataContainer.HorseModelPaths.Count);
                LoadHorse();

            }),
            right = new ButtonComponent.Entity(() =>
            {
                HorseId = ((HorseId + 1 + HorseMasterDataContainer.HorseModelPaths.Count) % HorseMasterDataContainer.HorseModelPaths.Count);
                LoadHorse();
            }),
            race = new ButtonComponent.Entity(() =>
            {
                UiLoadingPresenter.ShowLoadingAsync().Forget();
                this.Machine.ChangeState<HorseRaceActionState>();
            })
        });
        uiHorsePicker.In().Forget();
    }

    private void LoadHorse()
    {
        uiHorsePicker.horseLoader.SetEntity(new HorseLoader.Entity()
        {
            horse = HorseMasterDataContainer.HorseModelPaths[HorseId]
        });
    }

    public override void Exit()
    {
        base.Exit();
        cts?.Cancel();  
        cts = default;
        UiHeaderPresenter.HideHeader();
        UILoader.SafeRelease(ref uiHorsePicker);
    }
}
