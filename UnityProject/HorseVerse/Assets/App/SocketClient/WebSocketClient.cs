using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

public interface ISocketClient : IDisposable
{
    UniTask Connect(string url, int port);
    UniTask Send<T>(T message) where T : IMessage;
    UniTask<TResponse> Send<TRequest, TResponse>(TRequest request) where TRequest : IMessage
                                                                   where TResponse : IMessage;
    void Subscribe<T>(Action<T> callback) where T : IMessage;
    void UnSubscribe<T>(Action<T> callback) where T : IMessage;
}

public class WebSocketClient : SocketClientBase, ISocketClient
{
    private WebSocket ws;

    public static async UniTask<WebSocketClient> Initialize(string url, int port)
    {
        var go = new GameObject("WebSocketClient");
        var webSocketClient = go.AddComponent<WebSocketClient>();
        await webSocketClient.Connect(url, port);
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
    }

    public void UnSubscribeMessage()
    {
        ws.OnMessage -= OnMessage;
        ws.OnError -= OnError;
        ws.OnClose -= OnClose;
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
        ws = new WebSocket($"{url}:{port}");
        SubscribeMessage();
        await ws.Connect();
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
        GameObject.Destroy(gameObject);
        UnSubscribeMessage();
        await ws.Close();
        ws = default;
    }
}