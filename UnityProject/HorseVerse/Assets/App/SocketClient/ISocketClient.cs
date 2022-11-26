using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Google.Protobuf;

public interface ISocketClient : IDisposable
{
    UniTask Connect(string url, int port);
    UniTask Send<T>(T message) where T : IMessage;
    UniTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken token = default(CancellationToken)) where TRequest : IMessage
        where TResponse : IMessage;
    UniTask<TResponse> Send<TRequest, TResponse>(TRequest request, float TimeOut,CancellationToken token = default(CancellationToken)) where TRequest : IMessage
        where TResponse : IMessage;
    void Subscribe<T>(Action<T> callback) where T : IMessage;
    void UnSubscribe<T>(Action<T> callback) where T : IMessage;
}