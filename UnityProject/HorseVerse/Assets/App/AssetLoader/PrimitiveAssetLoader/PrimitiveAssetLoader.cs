using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Cysharp.Threading.Tasks;

public class PrimitiveAssetLoader
{
    private static Dictionary<string, (AsyncOperationHandle operationHandle, int refCount)> operationHandles = new Dictionary<string, (AsyncOperationHandle operationHandle, int refCount)>();

    public static async UniTask<T> LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        var handle = PrimitiveAssetLoader.GetOrCreatOperationHandle<T>(path);
        if (!handle.IsDone)
        {
            await handle.Task;
        }
        return (T)handle.Result;
    }

    private static AsyncOperationHandle GetOrCreatOperationHandle<T>(string path) where T : UnityEngine.Object
    {
        if (!operationHandles.TryGetValue(path, out var handler))
        {
            var handle = Addressables.LoadAssetAsync<T>(path);
            var refCount = 0;
            handler = (handle, refCount);
            operationHandles.Add(path, handler);
        }
        handler.refCount++;
        return handler.operationHandle;
    }

    public static void UnloadAssetAtPath(string path)
    {
        if (!operationHandles.TryGetValue(path, out var handler))
        {
            throw new Exception($"No asset loaded at path {path}");
        }
        handler.refCount--;
        if (handler.refCount == 0)
        {
            Addressables.Release(handler.operationHandle);
            operationHandles.Remove(path);
        }
    }
}
