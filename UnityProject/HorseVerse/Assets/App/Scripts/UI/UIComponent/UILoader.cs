using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class UILoader
{
    public static async UniTask<T> Load<T>(UICanvas.UICanvasType canvasType = UICanvas.UICanvasType.Default, CancellationToken token = default) where T : MonoBehaviour
    {
        var type = typeof(T).ToString();
        var prefab = await LoadResource<T>(type).AttachExternalCancellation(token) as T;
        var instance = GameObject.Instantiate<T>(prefab, UICanvas.GetCanvas(canvasType).transform, false);
        return instance;
    }

    private static async UniTask<Object> LoadResource<T>(string type) where T : MonoBehaviour
    {
        return await Resources.LoadAsync<T>($"UI/{type}");
    }

    public static void SafeUnload<T>(ref T ui) where T : MonoBehaviour
    {
        GameObject.Destroy(ui?.gameObject);
        ui = null;
    }
}
