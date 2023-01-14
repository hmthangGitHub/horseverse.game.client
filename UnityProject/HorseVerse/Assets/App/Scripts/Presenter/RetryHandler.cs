using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RetryHandler : IDisposable
{
    private readonly IDIContainer container;
    private LoginDomainService loginDomainService;
    private LoginDomainService LoginDomainService => loginDomainService ??= container.Inject<LoginDomainService>();
    private IPingDomainService pingDomainService;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private IPingDomainService PingDomainService => pingDomainService ??= container.Inject<IPingDomainService>();
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private ISocketClient socketClient;
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();

    private RetryHandler(IDIContainer container)
    {
        this.container = container;
        SocketClient.OnReconnect += OnReconnect;
    }

    public static RetryHandler Instantiate(IDIContainer container)
    {
        return new RetryHandler(container);
    }

    private async UniTask OnReconnect()
    {
        if (!await LoginDomainService.LoginWithAccessToken())
        {
            throw new Exception("Failed to retry connect by login with access token");
        }
        await UITouchDisablePresenter.Delay(1.0f);
        PingDomainService.StartPingService().Forget();
    }

    public void Dispose()
    {
        SocketClient.OnReconnect -= OnReconnect;
        socketClient = default;
        loginDomainService = default;
        pingDomainService = default;
        uiTouchDisablePresenter = default;
    }
}
