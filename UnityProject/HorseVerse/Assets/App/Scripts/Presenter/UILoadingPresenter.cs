using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UILoadingPresenter : IDisposable
{
    private UILoading uiLoading = default;
    private CancellationTokenSource cts = default;

    public async UniTask ShowLoadingAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiLoading ??= await UILoader.Load<UILoading>(UICanvas.UICanvasType.Loading).AttachExternalCancellation(cancellationToken: cts.Token);
        uiLoading.SetEntity(new UILoading.Entity());
        await uiLoading.In();
    }

    public void HideLoading()
    {
        cts.SafeCancelAndDispose();
        uiLoading?.Out().Forget();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        UILoader.SafeUnload(ref uiLoading);
    }
}