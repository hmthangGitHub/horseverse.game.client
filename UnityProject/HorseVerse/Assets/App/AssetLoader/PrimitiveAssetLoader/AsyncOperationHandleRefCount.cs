using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AsyncOperationHandleRefCount : MonoBehaviour
{
    private static Dictionary<string, (AsyncOperationHandle operationHandle, int refCount)> operationHandles = new Dictionary<string, (AsyncOperationHandle operationHandle, int refCount)>();
    public static void Add(string key, AsyncOperationHandle operationHandle)
    {
        operationHandles.Add(key, (operationHandle, 1));
    }

    public static bool TryGetHandler(string key,out AsyncOperationHandle handle)
    {
        if(operationHandles.TryGetValue(key, out var handler))
        {
            handle = handler.operationHandle;
            return true;
        }
        else
        {
            handle = default;
            return false;
        }
    }

    public static void IncreaseRef(string key)
    {
        (AsyncOperationHandle operationHandle, int refCount) handler = operationHandles[key];
        handler.refCount++;
        operationHandles[key] = handler;
    }

    public static bool DecreaseRef(string key)
    {
        (AsyncOperationHandle operationHandle, int refCount) handler = operationHandles[key];
        handler.refCount--;
        if (handler.refCount == 0)
        {
            operationHandles.Remove(key);
            return true;
        }
        else 
        {
            return false;
        }
    }
}
