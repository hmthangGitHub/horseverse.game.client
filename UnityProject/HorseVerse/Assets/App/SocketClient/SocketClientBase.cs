using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using UnityEngine;

public abstract class SocketClientBase : MonoBehaviour, ISocketClient
{
    protected MessageBroker.IMessageBroker messageBroker = new MessageBroker.ChannelMessageBroker();
    protected IMessageParser messageParser;

    public void SetIMessageParser(IMessageParser messageParser)
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

    public async UniTask<TResponse> Send<TRequest, TResponse>(TRequest request) where TRequest : IMessage
                                                                                where TResponse : IMessage
    {
        var ucs = new UniTaskCompletionSource<TResponse>();
        void OnResponse(TResponse response)
        {
            ucs.TrySetResult(response);
            messageBroker.UnSubscribe<TResponse>(OnResponse);
        }
        messageBroker.Subscribe<TResponse>(OnResponse);
        await Send<TRequest>(request);
        return await ucs.Task;
    }

    public abstract UniTask Connect(string url, int port);
    public abstract void Dispose();
    public abstract UniTask Send<T>(T message) where T : IMessage;
}