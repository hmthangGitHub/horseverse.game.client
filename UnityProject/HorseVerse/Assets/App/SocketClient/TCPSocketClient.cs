using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TCPSocketClient : SocketClientBase
{
    #region private members
    private NetworkStream stream;
	private CancellationTokenSource cancellationTokenSource;

	private TcpClient socketConnection;
    private int dataBufferSize = 4096;
    #endregion

    public static TCPSocketClient Initialize(IMessageParser messageParser, IErrorCodeConfiguration errorCodeConfig)
    {
        var go = new GameObject("TCPSocketClient");
        var tcpSocketClient = go.AddComponent<TCPSocketClient>();
        tcpSocketClient.Init(messageParser, errorCodeConfig);
        return tcpSocketClient;
    }

    public override async UniTask Send<T>(T message)
    {
        if (socketConnection.Connected)
        {
            await stream.SendRawMessage(messageParser.ToByteArray(message));
        }
    }

    public override void Dispose()
    {
        cancellationTokenSource.SafeCancelAndDispose();
        cancellationTokenSource = default;
        socketConnection?.Close();
        socketConnection?.Dispose();
        socketConnection = default;
        Destroy(this.gameObject);
    }

    public override async UniTask Connect(string url, int port)
    {
        cancellationTokenSource.SafeCancelAndDispose();
        cancellationTokenSource = new CancellationTokenSource();
        await ConnectTask(url, port).AttachExternalCancellation(cancellationTokenSource.Token).ThrowWhenTimeOut();
        ReadMessageAsync(socketConnection).Forget();
	}

    public override async UniTask Close()
    {
        cancellationTokenSource.SafeCancelAndDispose();
        cancellationTokenSource = default;
        socketConnection?.Close();
        socketConnection?.Dispose();
        socketConnection = default;
    }

    private async UniTask ConnectTask(string url, int port)
    {
        socketConnection = new TcpClient();
        await socketConnection.ConnectAsync(url, port);
    }

    private async UniTask ReadMessageAsync(TcpClient socketConnection)
    {
        stream = socketConnection.GetStream();
        var buffer = new byte[dataBufferSize];
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            var rawMessage = await stream.ReadRawMessage(buffer).AttachExternalCancellation(cancellationToken : cancellationToken);
            OnMessage(rawMessage);
        }
    }
}
