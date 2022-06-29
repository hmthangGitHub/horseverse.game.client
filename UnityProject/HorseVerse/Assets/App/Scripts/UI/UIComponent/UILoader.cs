using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class UILoader
{
    public static async UniTask<T> Instantiate<T>(UICanvas.UICanvasType canvasType = UICanvas.UICanvasType.Default, CancellationToken token = default) where T : PopupEntity
    {
        var type = typeof(T).ToString();
        var prefab = await LoadResource<T>(type).AttachExternalCancellation(token) as T;
        var instance = GameObject.Instantiate<T>(prefab, UICanvas.GetCanvas(canvasType).transform, false);
        return instance;
    }

    private static async UniTask<T> LoadResource<T>(string type) where T : PopupEntity
    {
        var go = await PrimitiveAssetLoader.LoadAsset<GameObject>(GetPathFromType(type));
        return go.GetComponent<T>();
    }

    private static string GetPathFromType(string type)
    {
        return $"UI/{type}";
    }

    public static void SafeRelease<T>(ref T ui) where T : PopupEntity
    {
        GameObject.Destroy(ui?.gameObject);
        ui = null;
        PrimitiveAssetLoader.UnloadAssetAtPath(GetPathFromType(typeof(T).ToString()));
    }
}
