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
        Debug.Log("Show Loading");
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiLoading ??= await UILoader.Instantiate<UILoading>(UICanvas.UICanvasType.Loading).AttachExternalCancellation(cancellationToken: cts.Token);
        uiLoading.SetEntity(new UILoading.Entity());
        await uiLoading.In();
    }

    public void HideLoading()
    {
        cts.SafeCancelAndDispose();
        uiLoading?.Out().Forget();
        Debug.Log("Hide Loading");
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        UILoader.SafeRelease(ref uiLoading);
    }
}