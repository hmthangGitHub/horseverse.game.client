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
        var prefab = await LoadResource<T>(type, token);
        var instance = GameObject.Instantiate<T>(prefab, UICanvas.GetCanvas(canvasType).transform, false);
        return instance;
    }

    public static async UniTask<T> InstantiateInSpace<T>(ObjectCanvas.UICanvasType canvasType = ObjectCanvas.UICanvasType.Default, CancellationToken token = default) where T : PopupEntity
    {
        var type = typeof(T).ToString();
        var prefab = await LoadResource<T>(type, token);
        var instance = GameObject.Instantiate<T>(prefab, ObjectCanvas.GetCanvas(canvasType).transform, false);
        return instance;
    }

    private static async UniTask<T> LoadResource<T>(string type, CancellationToken token) where T : PopupEntity
    {
        var go = await PrimitiveAssetLoader.LoadAssetAsync<GameObject>(GetPathFromType(type), token);
        if (go == default) Debug.LogError("Cant Load " + type);
        return go.GetComponent<T>();
    }

    private static string GetPathFromType(string type)
    {
        return $"UI/{type}";
    }

    public static void SafeRelease<T>(ref T ui) where T : PopupEntity
    {
        if (ui != null)
        {
            Object.Destroy(ui.gameObject);
            ui = null;
            PrimitiveAssetLoader.UnloadAssetAtPath(GetPathFromType(typeof(T).ToString()));
        }
    }
    
    public static void SafeRelease<T>(ref T[] uis) where T : PopupEntity
    {
        for (int i = 0; i < uis.Length; i++)
        {
            var ui = uis[i];
            SafeRelease(ref ui);
        }
        uis = default;
    }
}
