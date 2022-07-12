using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PrimitiveAssetLoader : AssetLoaderBase<PrimitiveAssetLoader>
{
    public static async UniTask<T> LoadAssetAsync<T>(string path, CancellationToken token) where T : UnityEngine.Object
    {
        var handle = Instance.GetOrCreatOperationHandle<T>(path);
        return await Instance.LoadAssetInternal<T>(path, handle, token);
    }

    protected override void ReleaseHandle(AsyncOperationHandle handle)
    {
        Addressables.Release(handle);
    }

    private AsyncOperationHandle GetOrCreatOperationHandle<T>(string path) where T : UnityEngine.Object
    {
        if (!AsyncOperationHandleRefCount.TryGetHandler(path, out var handle))
        {
            handle = Addressables.LoadAssetAsync<T>(path);
            AsyncOperationHandleRefCount.Add(path, handle);
        }
        else
        {
            AsyncOperationHandleRefCount.IncreaseRef(path);
        }
        return handle;
    }
}
