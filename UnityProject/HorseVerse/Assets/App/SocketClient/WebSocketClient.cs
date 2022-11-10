using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

public class WebSocketClient : SocketClientBase, ISocketClient
{
    private WebSocket ws;
    private CancellationTokenSource cancellationTokenSource;
    private UniTaskCompletionSource connectTask;

    public static WebSocketClient Initialize(IMessageParser messageParser)
    {
        var go = new GameObject("WebSocketClient");
        var webSocketClient = go.AddComponent<WebSocketClient>();
        webSocketClient.SetIMessageParser(messageParser);
        return webSocketClient;
    }

    public void SubscribeMessage()
    {
        ws.OnMessage += OnMessage;
        ws.OnError += OnError;
        ws.OnClose += OnClose;
        ws.OnOpen += OnOpen;
    }

    private void OnOpen()
    {
        Debug.Log("Open ws");
        connectTask.TrySetResult();
    }

    public void UnSubscribeMessage()
    {
        ws.OnMessage -= OnMessage;
        ws.OnError -= OnError;
        ws.OnClose -= OnClose;
        ws.OnOpen += OnOpen;
    }

    private void OnClose(WebSocketCloseCode closeCode)
    {
        Debug.LogError("Ws close with code " + closeCode);
    }

    private void OnError(string errorMsg)
    {
        Debug.LogError("Ws Error :" + errorMsg);
    }

    public override async UniTask Send<T>(T message)
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.Send(messageParser.ToByteArray(message));
        }
        await UniTask.CompletedTask;
    }

    public override async UniTask Connect(string url, int port)
    {
        cancellationTokenSource.SafeCancelAndDispose();
        cancellationTokenSource = new CancellationTokenSource();
        connectTask = new UniTaskCompletionSource();
        ws = new WebSocket("ws://tcp.prod.game.horsesoflegends.com:8769/ws");
        // ws = new WebSocket($"{url}:{port}");
        SubscribeMessage();
        _ = ws.Connect();
        await connectTask.Task.AttachExternalCancellation(cancellationTokenSource.Token);
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws?.State == WebSocketState.Open)
        {
            ws.DispatchMessageQueue();
        }
#endif
    }

    public override async void Dispose()
    {
        cancellationTokenSource.SafeCancelAndDispose();
        cancellationTokenSource = default;
        GameObject.Destroy(gameObject);
        UnSubscribeMessage();
        await ws.Close();
        ws = default;
    }
}