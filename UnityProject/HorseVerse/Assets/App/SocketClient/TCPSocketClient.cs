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
    private int dataBufferSize = 1024;
    #endregion

    public static async UniTask<TCPSocketClient> Initialize(string host, int port)
    {
        var go = new GameObject("TCPSocketClient");
        var tcpSocketClient = go.AddComponent<TCPSocketClient>();
        await tcpSocketClient.Connect(host, port);
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
        socketConnection.Close();
        socketConnection.Dispose();
    }

    public override async UniTask Connect(string url, int port)
    {
		try
        {
            socketConnection = new TcpClient();
            await socketConnection.ConnectAsync(url, port);
            ReadMessageAsync(socketConnection).Forget();
        }
        catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
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
            await stream.ReadRawMessage(buffer).AttachExternalCancellation(cancellationToken : cancellationToken);
            OnMessage(buffer);
        }
    }
}
