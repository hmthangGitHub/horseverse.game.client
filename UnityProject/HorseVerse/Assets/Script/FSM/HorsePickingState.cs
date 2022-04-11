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
    private string[] horseMasterData = new string[]
    {
        "Horses/Horse_Black",
        "Horses/Horse_Black_Tobiano_pinto",
        "Horses/Horse_Brown",
        "Horses/Horse_Buckskin",
        "Horses/Horse_GraysRoans",
        "Horses/Horse_Palomino",
        "Horses/Horse_palomino_overo_pinto",
        "Horses/Horse_White",
    };

    public int HorseIndex { get; private set; }

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
                horse = horseMasterData[HorseIndex]
            },
            left = new ButtonComponent.Entity(() =>
            {
                HorseIndex = ((HorseIndex - 1 + horseMasterData.Length) % horseMasterData.Length);
                LoadHorse();

            }),
            right = new ButtonComponent.Entity(() =>
            {
                HorseIndex = ((HorseIndex + 1 + horseMasterData.Length) % horseMasterData.Length);
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
            horse = horseMasterData[HorseIndex]
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
