using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class AssetLoaderBase
{ 
}

public abstract class AssetLoaderBase<T> : AssetLoaderBase where T : AssetLoaderBase<T>, new()
{
    private static T instance = default;
    protected static T Instance => instance ??= new T();

    public static void UnloadAssetAtPath(string path)
    {
        if (!AsyncOperationHandleRefCount.TryGetHandler(path, out var handle))
        {
            throw new Exception($"No asset loaded at path {path}");
        }
        if (AsyncOperationHandleRefCount.DecreaseRef(path))
        {
            Instance.ReleaseHandle(handle);
        }
    }

    protected abstract void ReleaseHandle(AsyncOperationHandle handle);

    protected async UniTask<T> LoadAssetInternal<T>(string path, AsyncOperationHandle handle, CancellationToken token)
    {
        if (!handle.IsDone)
        {
            try
            {
                await handle.Task.AsUniTask().AttachExternalCancellation(token);
            }
            catch (OperationCanceledException e)
            {
                UnloadAssetAtPath(path);
                throw;
            }
        }
        return (T)handle.Result;
    }
}