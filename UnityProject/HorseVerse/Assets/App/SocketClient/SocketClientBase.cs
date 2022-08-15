using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Threading;
using UnityEngine;

public abstract class SocketClientBase : MonoBehaviour, ISocketClient
{
    private MessageBroker.IMessageBroker messageBroker = new MessageBroker.ChannelMessageBroker();
    protected IMessageParser messageParser;

    protected void SetIMessageParser(IMessageParser messageParser)
    {
        this.messageParser = messageParser;
    }

    protected void OnMessage(byte[] data)
    {
        var message = messageParser.Parse(data);
        Debug.Log("Received Response" + message);
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
            ucs.TrySetResult(response);
            Debug.Log("Received response " + response);
        }
        messageBroker.Subscribe<TResponse>(OnResponse);
        await Send<TRequest>(request);
        try
        {
            if (token == default)
            {
                return await ucs.Task.ThrowWhenTimeOut();
            }
            else
            {
                return await ucs.Task.AttachExternalCancellation(token);
            }
        }
        finally
        {
            messageBroker.UnSubscribe<TResponse>(OnResponse);
        }
        
    }

    public abstract UniTask Connect(string url, int port);
    public abstract void Dispose();
    public abstract UniTask Send<T>(T message) where T : IMessage;
}