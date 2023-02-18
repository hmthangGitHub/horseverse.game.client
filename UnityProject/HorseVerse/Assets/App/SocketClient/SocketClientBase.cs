using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

public abstract class SocketClientBase : MonoBehaviour, ISocketClient
{
    private readonly MessageBroker.IMessageBroker messageBroker = new MessageBroker.ChannelMessageBroker();
    protected IMessageParser messageParser;
    protected IErrorCodeConfiguration errorCodeConfig;
    public event Action OnStartRequest = ActionUtility.EmptyAction.Instance;
    public event Action OnEndRequest = ActionUtility.EmptyAction.Instance;

    protected void Init(IMessageParser messageParser, IErrorCodeConfiguration errorCodeConfig)
    {
        this.messageParser = messageParser;
        this.errorCodeConfig = errorCodeConfig;
    }

    protected void OnMessage(byte[] data)
    {
        var message = messageParser.Parse(data);
        Debug.Log("Received response " + message);
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

    public async UniTask<TResponse> Send<TRequest, TResponse>(TRequest request, float timeOut = 10.0f, CancellationToken token = default(CancellationToken)) where TRequest : IMessage
                                                                                where TResponse : IMessage
    {
        Debug.Log($"Sending request {request.GetType()} {request}");
        var ucs = new UniTaskCompletionSource<TResponse>();
        var cts = new CancellationTokenSource();
        void OnResponse(TResponse response)
        {
            try
            {
                VerifyErrorMessage(response);
                ucs.TrySetResult(response);
            }
            catch
            {
                cts.SafeCancelAndDispose();
                throw;
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
                ? await ucs.Task.ThrowWhenTimeOut(timeOut, cts.Token)
                : await ucs.Task.AttachExternalCancellation(token);
        }
        finally
        {
            messageBroker.UnSubscribe<TResponse>(OnResponse);
            cts.SafeCancelAndDispose();
            if (token == default)
            {
                OnEndRequest.Invoke();
            }
        }
    }

    private void VerifyErrorMessage<TResponse>(TResponse response) where TResponse : IMessage
    {
        if (response is IErrorCodeMessage errorCodeMessage
            && errorCodeMessage.ResultCode != this.errorCodeConfig.SuccessCode
            && !errorCodeConfig.HandleCode.Contains(errorCodeMessage.ResultCode))
        {
            var message = errorCodeConfig.ErrorCodeMessage.TryGetValue(errorCodeMessage.ResultCode, out var msg)
                ? msg
                : "Unknown Message";
            throw new Exception($"Failed Response Exception Result Code:{errorCodeMessage.ResultCode} " +
                                $"- {message} \n" +
                                $"{response}");
        }
    }

    public abstract UniTask Connect(string url, int port);
    public abstract UniTask Close();
    public abstract void Dispose();
    public abstract UniTask Send<T>(T message) where T : IMessage;
}