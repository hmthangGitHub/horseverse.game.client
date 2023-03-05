using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class StreamExtensions
{
    public static async UniTask<byte[]> ReadRawMessage(this Stream stream, IMessageParser messageParser)
    {
        await UniTask.SwitchToTaskPool();
        
        var (size, sizeMessageArray) = await messageParser.GetNextMessageSize(stream);
        var buffer = new byte [size];
        
        var length = 0;
        while (length == 0)
        {
            length = await stream.ReadAsync(buffer, 0, size).ConfigureAwait(false);
        }
        
        await UniTask.SwitchToMainThread();
        return buffer;
    }

    public static async UniTask SendRawMessage(this Stream stream, byte[] data)
    {
        await UniTask.SwitchToTaskPool();
        await Task.WhenAll(stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, 0), stream.WriteAsync(data, 0, data.Length));
        await UniTask.SwitchToMainThread();
    }
}
