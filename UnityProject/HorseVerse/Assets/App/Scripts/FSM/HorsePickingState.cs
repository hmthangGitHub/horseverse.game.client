using RobustFSM.Base;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class HorsePickingState : InjectedBState
{
    private UIHorsePicker uiHorsePicker = default;
    private CancellationTokenSource cts = default;
    public int HorseId { get; private set; }
    public UILoadingPresenter uiLoadingPresenter;
    public override async void Enter()
    {
        base.Enter();
        uiLoadingPresenter = this.Container.Inject<UILoadingPresenter>();
        uiLoadingPresenter.HideLoading();

        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiHorsePicker ??= await UILoader.Load<UIHorsePicker>(token: cts.Token);
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
                uiLoadingPresenter.ShowLoadingAsync().Forget();
                this.Machine.ChangeState<HorseRaceState>();
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
        GameObject.Destroy(uiHorsePicker.gameObject);
        uiHorsePicker = null;
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
