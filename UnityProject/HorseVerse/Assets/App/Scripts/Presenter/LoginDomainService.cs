using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public class LoginDomainService
{
    private readonly IDIContainer container;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private ISocketClient socketClient;

    private LoginDomainService(IDIContainer container)
    {
        this.container = container;
    }

    public static LoginDomainService Instantiate(IDIContainer container)
    {
        return new LoginDomainService(container);
    }

    public async UniTask<bool> LoginWithAccessToken()
    {
#if MULTI_ACCOUNT
        var indexToken = PlayerPrefs.GetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, "");
        var token = PlayerPrefs.GetString(GameDefine.TOKEN_STORAGE + indexToken, "");
#else
        var token = PlayerPrefs.GetString(GameDefine.TOKEN_STORAGE, "");
#endif
        if (!string.IsNullOrEmpty(token))
        {
            var res = await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
            {
                LoginType = LoginType.AccessToken,
                ClientInfo = new io.hverse.game.protogen.ClientInfo()
                {
                    AccessToken = token,
                    Platform = LoginStatePresenter.GetCurrentPlatform(),
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    Model = SystemInfo.deviceModel,
                    Version = Application.version,
                }
            }, isHighPriority: true);
            return res.ResultCode == 100;
        }
        return false;
    }
}
