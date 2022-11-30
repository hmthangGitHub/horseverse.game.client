﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class UITouchDisablePresenter : IDisposable
{
    private readonly IDIContainer container;
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    
    private CancellationTokenSource cts;
    private UITouchDisable uiTouchDisable;

    private UITouchDisablePresenter(IDIContainer container)
    {
        this.container = container;
        SocketClient.OnStartRequest += OnStartRequest;
        SocketClient.OnEndRequest += OnEndRequest;
    }

    private void OnEndRequest()
    {
        Hide().Forget();
    }

    private void OnStartRequest()
    {
        ShowAsync().Forget();
    }

    public static async UniTask<UITouchDisablePresenter> InstantiateAsync(IDIContainer container)
    {
        var presenter = new UITouchDisablePresenter(container);
        presenter.uiTouchDisable = await UILoader.Instantiate<UITouchDisable>(UICanvas.UICanvasType.Loading);
        presenter.uiTouchDisable.SetEntity(new UITouchDisable.Entity());
        return presenter;
    }

    public async UniTask ShowAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await uiTouchDisable.In();
    }

    public async UniTask ShowTillFinishTaskAsync(UniTask task)
    {
        await ShowAsync();
        await task;
        await Hide();
    }

    public async UniTask Delay(float second)
    {
        await ShowTillFinishTaskAsync(UniTask.Delay(TimeSpan.FromSeconds(second)));
    }
    
    public async UniTask Hide()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await uiTouchDisable.Out();
    }
    
    public void Dispose()
    {
        SocketClient.OnStartRequest -= OnStartRequest;
        SocketClient.OnEndRequest -= OnEndRequest;
        socketClient = default;
        
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiTouchDisable);
    }
}