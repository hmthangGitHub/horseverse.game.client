using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIMainMenuPresenter : IDisposable
{
    public UIMainMenu uiMainMenu;
    private CancellationTokenSource cts;

    public event Action OnBetModeBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnBreedingBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnInventoryBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnLibraryBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnPlayBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnStableBtn = ActionUtility.EmptyAction.Instance;

    public async UniTaskVoid ShowMainMenuAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiMainMenu ??= await UILoader.Load<UIMainMenu>(token: cts.Token);
        uiMainMenu.SetEntity(new UIMainMenu.Entity()
        {
            betmodeBtn = new ButtonComponent.Entity(OnBetModeBtn),
            breedingBtn = new ButtonComponent.Entity(OnBreedingBtn),
            inventoryBtn = new ButtonComponent.Entity(OnInventoryBtn),
            libraryBtn = new ButtonComponent.Entity(OnLibraryBtn),
            playBtn = new ButtonComponent.Entity(OnPlayBtn),
            stableBtn = new ButtonComponent.Entity(OnStableBtn)
        });
        uiMainMenu.In().Forget();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeUnload(ref uiMainMenu);
    }
}
