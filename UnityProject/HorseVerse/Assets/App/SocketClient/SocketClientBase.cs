﻿using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Threading;
using UnityEngine;

public abstract class SocketClientBase : MonoBehaviour, ISocketClient
{
    private readonly MessageBroker.IMessageBroker messageBroker = new MessageBroker.ChannelMessageBroker();
    protected IMessageParser messageParser;
    public event Action OnStartRequest = ActionUtility.EmptyAction.Instance;
    public event Action OnEndRequest = ActionUtility.EmptyAction.Instance;

    protected void SetIMessageParser(IMessageParser messageParser)
    {
        this.messageParser = messageParser;
    }

    protected void OnMessage(byte[] data)
    {
        var message = messageParser.Parse(data);
        messageBroker.Publish(message);
    }


    public void Subscribe<T>(Action<T> callback) where T : IMessage
    {
        messageBroker.Subscribe(callback);
    }

    public void UnSubscribe<T>(Action<T> callback) where T : IMessage
    {
        messageBroker.UnSubscribe(callback);
    }

    public async UniTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken token = default(CancellationToken)) where TRequest : IMessage
                                                                                where TResponse : IMessage
    {
        Debug.Log($"Sending request {request.GetType()} {request}");
        var ucs = new UniTaskCompletionSource<TResponse>();
        void OnResponse(TResponse response)
        {
            Debug.Log("Received response " + response);
            ucs.TrySetResult(response);
            if (token == default)
            {
                OnEndRequest.Invoke();
            }
        }
        messageBroker.Subscribe<TResponse>(OnResponse);
        if (token == default)
        {
            OnStartRequest.Invoke();
        }
        await Send<TRequest>(request);
        try
        {
            return token == default
                ? await ucs.Task.ThrowWhenTimeOut()
                : await ucs.Task.AttachExternalCancellation(token);
        }
        finally
        {
            messageBroker.UnSubscribe<TResponse>(OnResponse);
        }
    }

    public async UniTask<TResponse> Send<TRequest, TResponse>(TRequest request, float timeOut, CancellationToken token = default(CancellationToken)) where TRequest : IMessage
                                                                                where TResponse : IMessage
    {
        Debug.Log($"Sending request {request.GetType()} {request}");
        var ucs = new UniTaskCompletionSource<TResponse>();
        void OnResponse(TResponse response)
        {
            Debug.Log("Received response " + response);
            ucs.TrySetResult(response);
        }
        messageBroker.Subscribe<TResponse>(OnResponse);
        await Send<TRequest>(request);
        try
        {
            return token == default
                ? await ucs.Task.ThrowWhenTimeOut(timeOut)
                : await ucs.Task.AttachExternalCancellation(token);
        }
        finally
        {
            messageBroker.UnSubscribe<TResponse>(OnResponse);
        }
    }

    public abstract UniTask Connect(string url, int port);
    public abstract UniTask Close();
    public abstract void Dispose();
    public abstract UniTask Send<T>(T message) where T : IMessage;
}