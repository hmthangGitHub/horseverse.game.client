using RobustFSM.Base;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class HorsePickingState : BState
{
    private UIHorsePicker uiHorsePicker = default;
    private CancellationTokenSource cts = default;
    public int HorseId { get; private set; }
    public override async void Enter()
    {
        base.Enter();
        cts?.Cancel();
        cts = new CancellationTokenSource();
        uiHorsePicker ??= await UILoader.Load<UIHorsePicker>().AttachExternalCancellation(cts.Token);
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
                this.SuperMachine.ChangeState<HorseRaceState>();
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
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
