using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.UI;
using ProtoClientInfo = io.hverse.game.protogen.ClientInfo;
using Platform = io.hverse.game.protogen.Platform;
using Crosstales.BWF.Model;
using Crosstales.BWF.Util;
using Crosstales.BWF.Manager;
using Crosstales.BWF;

public class LoginStatePresenter : IDisposable
{
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private UILoginOTP uiLoginOTP;
    private UILogin uiLogin;
    private UIPopupMessage uiPopupMessage;
    private UILoginSetName uiLoginSetName;

    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??=container.Inject<UILoadingPresenter>();

    private UserDataRepository userDataRepository;
    private UserDataRepository UserDataRepository => userDataRepository ??= container.Inject<UserDataRepository>();

    private ServerDefine serverDefine;
    private const string ClientVersionScripableObjectPath = "ClientInfo/ClientInfo";

    private int currentProfileIndex = 0;

    private ManagerMask BadwordManager = ManagerMask.BadWord;
    private ManagerMask DomManager = ManagerMask.Domain;
    private ManagerMask CapsManager = ManagerMask.Capitalization;
    private ManagerMask PuncManager = ManagerMask.Punctuation;
    private List<string> Sources = new List<string>() { "english", "_global", "_emoji", "iana" };

    bool HasChangeName = true;

    public LoginStatePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ConnectAndLoginAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        BWFManager.Load();

        await ConnectToServerAsync();
        await LoginAsync();
        await HandleSetNameAsync();
    }

    private async UniTask LoginAsync()
    {
        var kq = await DoLoginWithAccessToken();
        UILoadingPresenter.HideLoading();

        if (kq == false)
        {
            int type = 0;
            bool res = false;

            while (!res)
            {
                type = await DoSelectLoginType();
                if (type == 1)
                    res = await DoLoginWithOTP();
                else
                    res = await DoLoginWithEmail();
            }
        }

        await UniTask.Delay(1000, cancellationToken: cts.Token);
    }

    private async UniTask ConnectToServerAsync()
    {
        await DoLoadServerSetting();
#if UNITY_WEBGL || WEB_SOCKET
        string host = "ws://tcp.prod.game.horsesoflegends.com";
        int port = 8669;
#else
        string host = "tcp.prod.game.horsesoflegends.com";
        int port = 8670;
        if (serverDefine != default)
        {
            host = serverDefine.Production.Host;
            port = serverDefine.Production.Port;
        }
#endif
        await doInitLocalLocalization();
#if CUSTOM_SERVER
        var uiSV = await UILoader.Instantiate<UIPopUpServerSelection>(token: cts.Token);
        uiPopupMessage = await UILoader.Instantiate<UIPopupMessage>(UICanvas.UICanvasType.PopUp, token: cts.Token);
        bool wait = true;
        currentProfileIndex = 0;
        uiSV.SetEntity(new UIPopUpServerSelection.Entity()
        {
            cancelBtn = new ButtonComponent.Entity(() =>
            {
                wait = false;
                currentProfileIndex = uiSV.entity.CurrentProfileIndex;
            }),
            connectBtn = new ButtonComponent.Entity(() =>
            {
                host = uiSV.hostInput.inputField.text;
                port = System.Convert.ToInt32(uiSV.portInput.inputField.text);
                wait = false;
                currentProfileIndex = uiSV.entity.CurrentProfileIndex;
            }),
            hostInput = new UIComponentInputField.Entity() { defaultValue = host, interactable = true },
            portInput = new UIComponentInputField.Entity() { defaultValue = port.ToString(), interactable = true },
            CurrentProfileIndex = currentProfileIndex,
            serverDefine = this.serverDefine,
        });
        uiSV.In()
            .Forget();
        UILoadingPresenter.HideLoading();
        try
        {
            await UniTask.WaitUntil(() => wait == false, cancellationToken: cts.Token);
        }
        finally
        {
            UILoader.SafeRelease(ref uiSV);
        }

#endif

#if MULTI_ACCOUNT
        PlayerPrefs.SetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, currentProfileIndex.ToString());
#endif

#if UNITY_WEBGL || WEB_SOCKET
        await SocketClient.Connect(host, port);
#else
        await SocketClient.Connect(host, port);
#endif
    }

    private async UniTask<bool> LoginWithEmailAsync()
    {
        var res = await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
        {
            LoginType = LoginType.Email,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLogin.id.inputField.text,
                Password = uiLogin.passWord.inputField.text,
                Platform = GetCurrentPlatform(),
#if MULTI_ACCOUNT
                DeviceId = $"{SystemInfo.deviceUniqueIdentifier}_{currentProfileIndex}",
#else
                DeviceId = SystemInfo.deviceUniqueIdentifier,
#endif
                Model = SystemInfo.deviceModel,
                Version = GetClientVersion(),
            }
        }, 5.0f);
        return await HandleLoginResponse(res);
    }

    public static io.hverse.game.protogen.Platform GetCurrentPlatform()
    {
#if UNITY_WEBGL
        return io.hverse.game.protogen.Platform.Web;
#endif
#if UNITY_ANDROID
        return io.hverse.game.protogen.Platform.Android;
#endif
#if UNITY_IOS
        return io.hverse.game.protogen.Platform.Ios;
#endif
        return io.hverse.game.protogen.Platform.Web;
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiLogin);
        UILoader.SafeRelease(ref uiPopupMessage);
        if(uiLoginSetName != default) UILoader.SafeRelease(ref uiLoginSetName);
        if(uiLoginOTP != default) UILoader.SafeRelease(ref uiLoginOTP);
        uiLogin = default;
        uiLoginOTP = default;
        socketClient = default;
        uiLoadingPresenter = default;
        uiLoginSetName = default;
    }

    private async UniTask<bool> DoLoginWithAccessToken()
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
                    Platform = GetCurrentPlatform(),
#if MULTI_ACCOUNT
                    DeviceId = $"{SystemInfo.deviceUniqueIdentifier}_{currentProfileIndex}",
#else
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
#endif
                    Model = SystemInfo.deviceModel,
                    Version = GetClientVersion(),
                }
            }, 10.0f);

            if (res.ResultCode == MasterErrorCodeDefine.SUCCESS || res.ResultCode == MasterErrorCodeDefine.NEED_TO_UPDATE_CLIENT)
            {
                return await HandleLoginResponse(res);
            }

            return false;
        }
        return false;
    }

    private async UniTask<int> DoSelectLoginType()
    {
        var ui = await UILoader.Instantiate<UILoginSelection>(token: cts.Token);
        bool wait = true;
        int type = 0;
        ui.SetEntity(new UILoginSelection.Entity()
        {
            emailLoginBtn = new ButtonComponent.Entity(() => { type = 0; wait = false; }),
            otpLoginBtn = new ButtonComponent.Entity(() => { type = 1; wait = false; })
        });
        await ui.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref ui);
        return type;
    }

    private async UniTask<bool> DoLoginWithEmail()
    {
        uiLogin ??= await UILoader.Instantiate<UILogin>(token: cts.Token);
        bool closed = false;
        bool result = false;
        uiLogin.SetEntity(new UILogin.Entity()
        {
            id = new UIComponentInputField.Entity(),
            passWord = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() =>
            {
                result = true;
                closed = true;
            }),
            cancelBtn = new ButtonComponent.Entity(()=> {
                CloseAccount().Forget(); closed = true; result = false;
            }), 
            registerBtn = new ButtonComponent.Entity(()=> { 
                
            })
        });
        uiLogin.In().Forget();
        await UniTask.WaitUntil(() => closed == true);
        if (result)
            return await LoginWithEmailAsync();
        return false;
    }

    private async UniTask CloseAccount()
    {
        await uiLogin.Out();
        UILoader.SafeRelease(ref uiLogin);
        uiLogin = default;
    }

    private async UniTask<bool> DoLoginWithOTP()
    {
        uiLoginOTP ??= await UILoader.Instantiate<UILoginOTP>(token: cts.Token);
        bool closed = false;
        bool result = false;
        uiLoginOTP.SetEntity(new UILoginOTP.Entity()
        {
            id = new UIComponentInputField.Entity(),
            code = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() => { closed = true; result = true; }),
            cancelBtn = new ButtonComponent.Entity(() => { closeOTP().Forget(); closed = true; result = false; }),
            getCodeBtn = new ButtonComponent.Entity(() => GetCodeOTPAsync().Forget())
        });
        await uiLoginOTP.In();
        await UniTask.WaitUntil(() => closed == true);
        if(result) 
            return await LoginOTPAsync();
        return result;
    }

    private async UniTask closeOTP()
    {
        await uiLoginOTP.Out();
        UILoader.SafeRelease(ref uiLoginOTP);
        uiLoginOTP = default;
    }

    private async UniTask<bool> LoginOTPAsync()
    {
        var res = await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
        {
            LoginType = LoginType.LoginEmailCode,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLoginOTP.id.inputField.text,
                EmailCode = uiLoginOTP.code.inputField.text,
                Platform = GetCurrentPlatform(),
#if MULTI_ACCOUNT
                DeviceId = $"{SystemInfo.deviceUniqueIdentifier}_{currentProfileIndex}",
#else
                DeviceId = SystemInfo.deviceUniqueIdentifier,
#endif
                Model = SystemInfo.deviceModel,
                Version = GetClientVersion(),
            }
        });
        return await HandleLoginResponse(res);
    }

    public static string GetClientVersion()
    {
        return Resources.Load<ClientInfo>(ClientVersionScripableObjectPath).Version;
    }

    private async UniTask GetCodeOTPAsync()
    {
        var res = await SocketClient.Send<EmailCodeRequest, EmailCodeResponse>(new EmailCodeRequest()
        {
            Email = uiLoginOTP.id.inputField.text,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLoginOTP.id.inputField.text,
                Password = "",
                Platform = GetCurrentPlatform(),
#if MULTI_ACCOUNT
                DeviceId = $"{SystemInfo.deviceUniqueIdentifier}_{currentProfileIndex}",
#else
                DeviceId = SystemInfo.deviceUniqueIdentifier,
#endif
                Model = SystemInfo.deviceModel,
                Version = GetClientVersion(),
            }
        });
        
        if (res.ResultCode == MasterErrorCodeDefine.SUCCESS)
        {
            var uiConfirm = await UILoader.Instantiate<UIPopupMessage>(token: cts.Token);
            bool wait = true;

            uiConfirm.SetEntity(new UIPopupMessage.Entity()
            {
                title = "SUCCEED",
                message = "Code sent successfully, please check your mailbox",
                confirmBtn = new ButtonComponent.Entity(() => { wait = false; })
            });
            await uiConfirm.In();
            await UniTask.WaitUntil(() => wait == false, cancellationToken: cts.Token);
            UILoader.SafeRelease(ref uiConfirm);
        }
        else
        {
            await HandleLoginError(res.ResultCode, res.UpdateClientLink);
        }
    }

    private async UniTask<bool> HandleLoginResponse(LoginResponse res)
    {
        if (res.ResultCode == MasterErrorCodeDefine.SUCCESS)
        {
            await LoginSucessAsync(res);
            return true;
        }
        else
        {
            return await HandleLoginError(res.ResultCode, res.UpdateClientLink);
        }
    }

    private async UniTask<bool> HandleLoginError(int resResultCode,
                                                 string resUpdateClientLink)
    {
        if (resResultCode == MasterErrorCodeDefine.NEED_TO_UPDATE_CLIENT)
        {
            ShowNewVersionPopUp(resUpdateClientLink);
            throw new OperationCanceledException("Client need update");
        }
        else
        {
            await ShowMessagePopUp("NOTICE", LanguageManager.GetText($"RESULT_CODE_{resResultCode}"));
            return false;
        }
    }

    private async UniTask<bool> HandleLoginError(int resResultCode)
    {
        await ShowMessagePopUp("NOTICE", LanguageManager.GetText($"RESULT_CODE_{resResultCode}"));
        return false;
    }

    private async UniTask<bool> HandleLoginError(string message)
    {
        await ShowMessagePopUp("NOTICE", message);
        return false;
    }

    private async Task LoginSucessAsync(LoginResponse res)
    {
        // GetMasterData 
        var masterData = await GetMasterData();
        UpdateMasterData(masterData);
        await UserDataRepository.UpdateDataAsync(new[] { res.PlayerInfo });
        var featurePresent = this.container.Inject<FeaturePresenter>();
        featurePresent.SetFeatureList(GetFeatureList(res));
#if MULTI_ACCOUNT
            var indexToken = PlayerPrefs.GetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, "");
            PlayerPrefs.SetString(GameDefine.TOKEN_STORAGE + indexToken, res.PlayerInfo.AccessToken);
#else
        PlayerPrefs.SetString(GameDefine.TOKEN_STORAGE, res.PlayerInfo.AccessToken);
#endif
        this.HasChangeName = res.HasChangedName;
    }

    private void ShowNewVersionPopUp(string updateLink)
    {
        uiPopupMessage.SetEntity(new UIPopupMessage.Entity()
        {
            message = "There is a new version available. Please download the new version to experience more features",
            title = "NOTICE",
            confirmBtn = new ButtonComponent.Entity(() => Application.OpenURL(updateLink))
        });
        uiPopupMessage.In().Forget();
    }

    private async UniTask ShowMessagePopUp(string title, string message)
    {
        var uiConfirm = await UILoader.Instantiate<UIPopupMessage>(token: cts.Token);
        bool wait = true;

        uiConfirm.SetEntity(new UIPopupMessage.Entity()
        {
            title = title,
            message = message,
            confirmBtn = new ButtonComponent.Entity(() => { wait = false; })
        });
        await uiConfirm.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref uiConfirm);
    }

    private async UniTask<MasterDataResponse> GetMasterData()
    {
        var res = await SocketClient.Send<MasterDataRequest, MasterDataResponse>(new MasterDataRequest());
        return res;
    }

    IEnumerator doInitLocalLocalization(System.Action onFinish = null)
    {
        string path = $"Localization/Localization";
        ResourceRequest rq = Resources.LoadAsync<LanguageDependentText>(path);
        yield return rq;
        if (rq.asset != null)
        {
            LanguageDependentText depen = rq.asset as LanguageDependentText;
            LanguageManager.InitializeLocal(SystemLanguage.English, depen);
        }
        onFinish?.Invoke();
    }

    IEnumerator DoLoadServerSetting()
    {
#if UNITY_WEBGL || WEB_SOCKET
        var rq = Resources.LoadAsync<ServerDefine>("Settings/WSServerDefine");
        yield return rq;
        if(rq.asset != null)
        {
            serverDefine = (ServerDefine)rq.asset;
        }
#else
        var rq = Resources.LoadAsync<ServerDefine>("Settings/TCPServerDefine");
        yield return rq;
        if(rq.asset != null)
        {
            serverDefine = (ServerDefine)rq.asset;
        }
#endif
    }

    private void UpdateMasterData(MasterDataResponse data)
    {
        UserSettingLocalRepository.MasterDataModel.MaxHappinessNumber = data.MaxHappinessNumber;
        UserSettingLocalRepository.MasterDataModel.TrainingHappinessCost = data.HappinessNumberPerTraining;
        UserSettingLocalRepository.MasterDataModel.BetNumberList.Clear();
        UserSettingLocalRepository.MasterDataModel.BetNumberList.AddRange(data.BetNumberList);
        UserSettingLocalRepository.MasterDataModel.MaxDailyRacingNumber = data.MaxDailyRacingNumber;
        UserSettingLocalRepository.MasterDataModel.RacingRewardInfos = data.RaceRewardInfos
                                                                           .ToDictionary(x => ((RacingRoomType)(int)x.Level, x.Rank), x => x.Rewards.ToArray());
    }

    private FEATURE_TYPE[] GetFeatureList(LoginResponse res)
    {
        List<FEATURE_TYPE> listFeature = new List<FEATURE_TYPE>();
        foreach(var item in res.FeatureList)
        {
            listFeature.Add((FEATURE_TYPE)item);
        }
        return listFeature.ToArray();
    }

    private async UniTask HandleSetNameAsync()
    {
        if (this.HasChangeName) return;
        uiLoginSetName ??= await UILoader.Instantiate<UILoginSetName>(token: cts.Token);
        var ucs = new UniTaskCompletionSource();
        uiLoginSetName.SetEntity(new UILoginSetName.Entity()
        {
            username = "",
            onUpdateInput = OnChangeNameValueChanged,
            confirmBtn = new ButtonComponent.Entity(()=>OnChangeNameClicked(ucs).Forget(), false)
        });
        await uiLoginSetName.In();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    private async UniTask OnChangeNameClicked(UniTaskCompletionSource ucs)
    {
        uiLoginSetName.SetInteractable(false);
        var newName = uiLoginSetName.username.text;
        if (isValidName(newName))
        {
            var res = await SocketClient.Send<ChangeNameRequest, ChangeNameResponse>(new ChangeNameRequest()
            {
                NewName = newName
            });

            if (res.ResultCode == MasterErrorCodeDefine.SUCCESS)
            {
                await UserDataRepository.UpdateName(newName);
                ucs.TrySetResult();
            }
            else
            {
                await HandleLoginError(res.ResultCode);
            }
        }
        else
        {
            await HandleLoginError(LanguageManager.GetText("NOT_VALID_NAME"));
        }
        uiLoginSetName.SetInteractable(true);
        OnChangeNameValueChanged(newName);
    }

    private void OnChangeNameValueChanged(string value)
    {
        if (uiLoginSetName == default) return;
        if (value.Trim().Length < 5) uiLoginSetName.confirmBtn.SetInteractable(false);
        else uiLoginSetName.confirmBtn.SetInteractable(true);
    }

    private bool isValidName(string _name)
    {
        var len = _name.Trim().Length;
        if (len >= 5 && len <= 16)
        {
            return !BWFManager.Contains(_name, BadwordManager | DomManager | CapsManager | PuncManager, Sources.ToArray());
        }
        return false;
    }
}