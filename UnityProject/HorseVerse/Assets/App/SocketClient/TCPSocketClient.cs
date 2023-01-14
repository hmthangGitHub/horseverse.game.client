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
        if (IsSocketConnected(socketConnection.Client))
        {
            await stream.SendRawMessage(messageParser.ToByteArray(message));
        }
    }
    
    static bool IsSocketConnected(Socket s)
    {
        return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        socketConnection?.Close();
        DisposeUtility.SafeDispose(ref socketConnection);
        
        Destroy(this.gameObject);
    }

    public override async UniTask Connect(string url, int port)
    {
        Url = url;
        Port = port;
        socketSessionCts.SafeCancelAndDispose();
        socketSessionCts = new CancellationTokenSource();
        await ConnectTask(url, port).AttachExternalCancellation(socketSessionCts.Token).ThrowWhenTimeOut();
        ReadMessageAsync(socketConnection).Forget();
	}

    public override async UniTask Close()
    {
        socketSessionCts.SafeCancelAndDispose();
        socketSessionCts = default;
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
        socketSessionCts?.Cancel();
        socketSessionCts = new CancellationTokenSource();
        var cancellationToken = socketSessionCts.Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            var rawMessage = await stream.ReadRawMessage(buffer).AttachExternalCancellation(cancellationToken : cancellationToken);
            OnMessage(rawMessage);
        }
    }
}
