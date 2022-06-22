using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class StreamExtensions
{
    public static async UniTask<byte[]> ReadRawMessage(this Stream stream, byte[] buffer)
    {
        await UniTask.SwitchToTaskPool();
        while (await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false) == 0) { }
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
