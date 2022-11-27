using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class DisposeUtility
{
    public static void SafeDispose<T>(ref T disposable) where T : IDisposable
    {
        if(disposable is Component component)
        {
            Object.Destroy(component?.gameObject);
        }
        disposable?.Dispose();
        disposable = default;
    }
}
