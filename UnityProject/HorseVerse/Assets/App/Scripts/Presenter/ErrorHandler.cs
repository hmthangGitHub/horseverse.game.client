using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class ErrorHandler : IDisposable
{
    private readonly IDIContainer container;
    private MasterLocalizeContainer masterLocalizeContainer;
    private MasterErrorCodeContainer masterErrorCodeContainer;
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private MasterLocalizeContainer MasterLocalizeContainer => masterLocalizeContainer ??= container.Inject<MasterLocalizeContainer>();
    private MasterErrorCodeContainer MasterErrorCodeContainer => masterErrorCodeContainer ??= container.Inject<MasterErrorCodeContainer>();
    public event Action OnError = ActionUtility.EmptyAction.Instance;
    private UIPopupMessage uiPopupMessage;
    private CancellationTokenSource cts;
#if ENABLE_DEBUG_MODULE
    private string Error => errorList[currentErrorIndex].error;
    private string StackTrace => errorList[currentErrorIndex].stackTrace;
    private readonly List<(string error, string stackTrace)> errorList = new List<(string error, string stackTrace)>();
    private int currentErrorIndex = 0;
#endif

    private ErrorHandler(IDIContainer container)
    {
        this.container = container;
        SubscribeEvents();
        cts = new CancellationTokenSource();
    }

    public static async UniTask<ErrorHandler> Instantiate(IDIContainer container)
    {
        var errorHandler = new ErrorHandler(container);
        await errorHandler.InitializeInternal();
        return errorHandler;
    }

    private async UniTask InitializeInternal()
    {
        uiPopupMessage = await UILoader.Instantiate<UIPopupMessage>(UICanvas.UICanvasType.Error, token: cts.Token);
    }

    private void SubscribeEvents()
    {
        Application.logMessageReceived += ApplicationLogMessageReceived;
    }

#if ENABLE_DEBUG_MODULE
    private Vector2 scrollPos;
    GUIStyle gsAlterQuest;
    GUIStyle areaStyle => gsAlterQuest ??= AreaGUIStyleUtil.CreateGs(new Color(0,0,0, 0.8f));

    private void OnGUIFunction()
    {
        if (!string.IsNullOrEmpty(Error))
        {
            GUI.skin.label.fontSize = 30;
            GUI.skin.button.fontSize = 30;
            GUI.backgroundColor = new Color(0, 0, 0, 1.0f);
            GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40), areaStyle);
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUI.backgroundColor = new Color(0, 0, 0, 1.0f);
            GUILayout.Label($"Error : {Error}");
            GUILayout.Label(StackTrace);
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"<{currentErrorIndex}"))
            {
                currentErrorIndex--;
                currentErrorIndex += errorList.Count;
                currentErrorIndex %= errorList.Count;
            }

            if (GUILayout.Button("Reload"))
            {
                OnError.Invoke();
            }

            if (GUILayout.Button("Copy error"))
            {
                GUIUtility.systemCopyBuffer = StackTrace;
            }

            if (GUILayout.Button("Quit"))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

            if (GUILayout.Button($"{errorList.Count - currentErrorIndex}>"))
            {
                currentErrorIndex++;
                currentErrorIndex %= errorList.Count;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
#endif

    private void ApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception) return;
        if (!condition.Contains("Exception")) return;
#if ENABLE_DEBUG_MODULE
        ShowError(condition, stackTrace);
#endif
        var message = condition switch
        {
            var socketException when socketException.Contains("SocketException") => "Network Error",
            var timeOutException when timeOutException.Contains("TimeoutException") => "Network Error",
            var failedResponseException when failedResponseException.Contains(nameof(FailedResponseException)) => MasterLocalizeContainer.GetString(GetDescriptionKey()),
            _ => "Unknown Error"
        };
        ShowErrorMessageAsync(message);
    }

    private string GetDescriptionKey()
    {
        return MasterErrorCodeContainer.MasterErrorCodeIndexer.TryGetValue(SocketClient.LatestException.ErrorCode, out var masterErrorCode)
                                       ? masterErrorCode.DescriptionKey
                                       : "UNKNOWN_ERROR";
    }

    private void ShowErrorMessageAsync(string message)
    {
        uiPopupMessage.SetEntity(new UIPopupMessage.Entity()
        {
            message = message,
            title = "ERROR",
            confirmBtn = new ButtonComponent.Entity(() => OnError.Invoke())
        });
        uiPopupMessage.In().Forget();
    }

    private void ShowError(string condition, string stackTrace)
    {
#if ENABLE_DEBUG_MODULE
        if(!errorList.Any())
        {
            UnityMessageForwarder.AddListener(UnityMessageForwarder.MessageType.OnGUI, OnGUIFunction);
        }
        if(errorList.Count < 999)
        {
            errorList.Add((condition, stackTrace));
        }
#endif
    }

    private void UnsubcribeEvents()
    {
#if ENABLE_DEBUG_MODULE
        UnityMessageForwarder.RemoveListener(UnityMessageForwarder.MessageType.OnGUI, OnGUIFunction);
#endif
        Application.logMessageReceived -= ApplicationLogMessageReceived;
    }

    public void Dispose()
    {
        UnsubcribeEvents();
        OnError = ActionUtility.EmptyAction.Instance;
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiPopupMessage);
#if ENABLE_DEBUG_MODULE
        errorList.Clear();
#endif
    }
}