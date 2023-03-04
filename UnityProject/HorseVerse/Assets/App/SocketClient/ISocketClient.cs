using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Google.Protobuf;

public interface ISocketClient : IDisposable
{
    UniTask Connect(string url, int port);
    UniTask Close();
    UniTask Send<T>(T message) where T : IMessage;
    UniTask<TResponse> Send<TRequest, TResponse>(TRequest request, float timeOut = 3.0f,CancellationToken token = default(CancellationToken), bool isHighPriority = false, bool retryWhenTimeOut = true) 
        where TRequest : IMessage
        where TResponse : IMessage;

    event Action OnStartRequest;
    event Action OnEndRequest;
    
    void Subscribe<T>(Action<T> callback) where T : IMessage;
    void UnSubscribe<T>(Action<T> callback) where T : IMessage;

    FailedResponseException LatestException { get; }
    
    event Func<UniTask> OnReconnect;

    string Url { get; }
    int Port { get; }
}