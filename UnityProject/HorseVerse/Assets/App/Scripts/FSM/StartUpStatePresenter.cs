using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StartUpStatePresenter : IDisposable
{
    private const string ClientVersionScripableObjectPath = "ClientInfo/ClientInfo";
    public event Action OnReboot = ActionUtility.EmptyAction.Instance;
    private UIClientInfo uiClientInfo;
    private CancellationTokenSource cts;
    
    public async UniTask ShowClientInfoAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiClientInfo ??= await UILoader.Instantiate<UIClientInfo>(UICanvas.UICanvasType.Info, token: cts.Token);
        uiClientInfo.SetEntity(new UIClientInfo.Entity()
        {
            clientVersion = Resources.Load<ClientInfo>(ClientVersionScripableObjectPath).Version
        });
        uiClientInfo.In().Forget();
    }
    
    public void Reboot()
    {
        OnReboot.Invoke();
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiClientInfo);
    }
}