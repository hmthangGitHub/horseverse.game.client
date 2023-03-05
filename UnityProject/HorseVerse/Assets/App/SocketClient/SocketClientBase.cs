using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SocketClientBase : MonoBehaviour, ISocketClient
{
    private readonly MessageBroker.IMessageBroker messageBroker = new MessageBroker.ChannelMessageBroker();
    protected IMessageParser messageParser;
    protected CancellationTokenSource socketSessionCts;
    protected CancellationTokenSource appSessionCts;
    protected IErrorCodeConfiguration errorCodeConfig;
    public FailedResponseException LatestException { get; private set; }
    private SemaphoreSlim readLock = new SemaphoreSlim(1, 1); 
    
    private int maxRequestTime = 4;
    public event Action OnStartRequest = ActionUtility.EmptyAction.Instance;
    public event Action OnEndRequest = ActionUtility.EmptyAction.Instance;
    private event Action OnParseMessageError = ActionUtility.EmptyAction.Instance;
    public event Func<UniTask> OnReconnect = () => UniTask.CompletedTask;
    public string Url { get; protected set; }
    public int Port { get; protected set; }
    private UniTaskCompletionSource ucs;

    protected void Init(IMessageParser messageParser, IErrorCodeConfiguration errorCodeConfig)
    {
        this.messageParser = messageParser;
        this.errorCodeConfig = errorCodeConfig;
        this.appSessionCts = new CancellationTokenSource();
    }

    protected void OnMessage(byte[] data)
    {
        IMessage message = default;
        try
        {
            message = messageParser.Parse(data);
            Debug.Log($"Received response {message.GetType()} {message}");
        }
        catch
        {
            OnParseMessageError.Invoke();
            throw;
        }
        
        if (!messageBroker.Any(message.GetType()))
        {
            VerifyErrorMessage(message);
        }
        else
        {
            messageBroker.Publish(message);
        }
    }
    
    public void Subscribe<T>(Action<T> callback) where T : IMessage
    {
        messageBroker.Subscribe(callback);
    }

    public void UnSubscribe<T>(Action<T> callback) where T : IMessage
    {
        messageBroker.UnSubscribe(callback);
    }

    public async UniTask<TResponse> Send<TRequest, TResponse>(TRequest request, float timeOut = 10.0f, CancellationToken token = default(CancellationToken), bool isHighPriority = false, bool retryWhenTimeOut = false) 
                                                                                where TRequest : IMessage
                                                                                where TResponse : IMessage
    {
        if (isHighPriority)
        {
            return await SendRequestInternal<TRequest, TResponse>(request, timeOut, token);
        }
        return await SendMessageWithRetry<TRequest, TResponse>(request, timeOut, token, retryWhenTimeOut);
    }

    private async UniTask<TResponse> SendMessageWithRetry<TRequest, TResponse>(TRequest request,
                                                                               float timeOut,
                                                                               CancellationToken token,
                                                                               bool retryWhenTimeOut) 
                                                                                where TRequest : IMessage 
                                                                                where TResponse : IMessage
    {
        if (token == default)
        {
            await readLock.WaitAsync(appSessionCts.Token);
        }

        try
        {
            for (var i = 0; i < maxRequestTime; i++)
            {
                try
                {
                    return await SendRequestInternal<TRequest, TResponse>(request, timeOut, token);
                }
                catch (TimeoutException)
                {
                    if (i != maxRequestTime - 1 && retryWhenTimeOut)
                    {
                        Debug.Log($"Retry {i + 1} time");
                        await Close();
                        await RetryConnectTask();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new Exception($"Send request failed {request}");
        }
        finally
        {
            if (token == default)
            {
                readLock.Release();
            }
        }
    }

    private async UniTask RetryConnectTask()
    {
        if (ucs == default)
        {
            ucs = new UniTaskCompletionSource();
            await Connect(Url, Port);
            await OnReconnect().AttachExternalCancellation(cancellationToken: socketSessionCts.Token);
            ucs.TrySetResult();
        }

        await ucs.Task.AttachExternalCancellation(cancellationToken: socketSessionCts.Token);
        ucs = default;
    }

    private async UniTask<TResponse> SendRequestInternal<TRequest, TResponse>(TRequest request,
                                                                           float timeOut,
                                                                           CancellationToken token) where TRequest : IMessage
        where TResponse : IMessage
    {
        Debug.Log($"Sending request {request.GetType()} {request}");
        
        var requestUcs = new UniTaskCompletionSource<TResponse>();
        var cts = new CancellationTokenSource();

        void OnResponse(TResponse response)
        {
            try
            {
                VerifyErrorMessage(response);
                requestUcs.TrySetResult(response);
            }
            catch
            {
                cts.SafeCancelAndDispose();
                throw;
            }
        }
        
        void OnParseMessageErrorInternal()
        {
            cts.SafeCancelAndDispose();
        }

        messageBroker.Subscribe<TResponse>(OnResponse);
        
        if (token == default)
        {
            OnStartRequest.Invoke();
        }

        await Send<TRequest>(request);
        OnParseMessageError += OnParseMessageErrorInternal;
        try
        {
            return token == default
                ? await requestUcs.Task.ThrowWhenTimeOut(timeOut, cts.Token)
                : await requestUcs.Task.AttachExternalCancellation(token);
        }
        finally
        {
            messageBroker.UnSubscribe<TResponse>(OnResponse);
            OnParseMessageError -= OnParseMessageErrorInternal;
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
            
            LatestException = new FailedResponseException($"Result Code:{errorCodeMessage.ResultCode} " +
                                                          $" - {message} \n" +
                                                          $" {response}", 
                errorCodeMessage.ResultCode);
            throw LatestException;
        }
    }

    public abstract UniTask Connect(string url, int port);
    public abstract UniTask Close();

    public virtual void Dispose()
    {
        DisposeUtility.SafeDispose(ref socketSessionCts);
        DisposeUtility.SafeDispose(ref appSessionCts);
        DisposeUtility.SafeDispose(ref readLock);
    }
    public abstract UniTask Send<T>(T message) where T : IMessage;
}