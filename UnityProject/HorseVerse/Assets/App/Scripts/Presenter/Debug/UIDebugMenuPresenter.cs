#if ENABLE_DEBUG_MODULE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class UIDebugMenuPresenter : IDisposable
{
    private bool isInvisibleDebugMenu;
    private IDIContainer Container { get; }

    private UIDebugMenu uiDebugMenu;
    private CancellationTokenSource cts;
    private readonly List<(string debugMenu, Action action)> debugMenuList = new List<(string debugMenu, Action)>();
    public event Action OnToLevelEditorState = ActionUtility.EmptyAction.Instance;
    public event Action OnToTrainingState = ActionUtility.EmptyAction.Instance;

    public UIDebugMenuPresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask InitializeAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiDebugMenu ??= await UILoader.Instantiate<UIDebugMenu>(UICanvas.UICanvasType.Debug, token: cts.Token);
        await CreateDebugMenusAsync();
        SetEntity();
    }

    private void SetEntity()
    {
        uiDebugMenu.SetEntity(new UIDebugMenu.Entity()
        {
            states = UIDebugMenuState.State.Collapse,
            closeBtn = new ButtonComponent.Entity(() => UpdateDebugMenuState(UIDebugMenuState.State.Collapse)),
            openBtn = new ButtonComponent.Entity(() => UpdateDebugMenuState(UIDebugMenuState.State.Expand)),
            debugMenuItemList = new UIDebugMenuItemList.Entity()
            {
                entities = CreateDebugMenuEntities()
            }
        });
        uiDebugMenu.In().Forget();
    }

    private UIDebugMenuItem.Entity[] CreateDebugMenuEntities()
    {
        return debugMenuList.Select(x => new UIDebugMenuItem.Entity()
        {
            debugMenu = x.debugMenu,
            debugMenuBtn = new ButtonComponent.Entity(x.action)
        }).ToArray();
    }

    private void UpdateDebugMenuState(UIDebugMenuState.State state)
    {
        uiDebugMenu.entity.states = state;
        uiDebugMenu.states.SetEntity(uiDebugMenu.entity.states);
    }
    
    private async UniTask CreateDebugMenusAsync()
    {
        debugMenuList.AddRange(await CreateMasterEditionDebugMenu());
        debugMenuList.Add(CreateReloadDebugMenu());
        debugMenuList.Add(CreateInvisibleDebugMenu());
        debugMenuList.Add(CreateLevelEditorDebugMenu());
        debugMenuList.Add(CreateToTrainingState());
    }

    private (string debugMenu, Action action) CreateToTrainingState()
    {
        return ("Transition/Training", () => OnToTrainingState.Invoke());
    }

    private (string debugMenu, Action action) CreateLevelEditorDebugMenu()
    {
        return ("Level Editor", () =>
        {
            UpdateDebugMenuState(UIDebugMenuState.State.Collapse);
            OnToLevelEditorState.Invoke();
        });
    }

    private (string debugMenu, Action action) CreateInvisibleDebugMenu()
    {
        return (GetCurrentInvisibleDebugMenu(), () =>
        {
            var entity = uiDebugMenu.entity.debugMenuItemList.entities.First(x => x.debugMenu.Equals(GetCurrentInvisibleDebugMenu()));
            isInvisibleDebugMenu = !isInvisibleDebugMenu;
            entity.debugMenu = GetCurrentInvisibleDebugMenu();
            
            uiDebugMenu.ShowDebugMenuAsVisible(isInvisibleDebugMenu);
            uiDebugMenu.RefreshMenu();
            if (isInvisibleDebugMenu)
            {
                UpdateDebugMenuState(UIDebugMenuState.State.Collapse);
            }
        });
    }

    public void AddDebugMenu(string debugMenu, Action action)
    {
        debugMenuList.Add((debugMenu, action));
        UpdateDebugMenu();
    }

    private void UpdateDebugMenu()
    {
        uiDebugMenu.entity.debugMenuItemList.entities = CreateDebugMenuEntities();
        uiDebugMenu.debugMenuItemList.SetEntity(uiDebugMenu.entity.debugMenuItemList.entities);
        uiDebugMenu.RefreshMenu();
    }

    public void RemoveDebugMenu(string debugMenu)
    {
        debugMenuList.RemoveAll(x => x.debugMenu == debugMenu);
        UpdateDebugMenu();
    }

    private string GetCurrentInvisibleDebugMenu()
    {
        return (isInvisibleDebugMenu ? "Debug Menu : Show" : "Debug Menu : Invisible");
    }

    private (string debugMenu, Action action) CreateReloadDebugMenu()
    {
        return ("Reload", () => throw new Exception("Reload"));
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiDebugMenu);
        debugMenuList.Clear();
    }
}
#endif