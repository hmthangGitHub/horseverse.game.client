using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoader
{
    public static async UniTask<T> Load<T>() where T : MonoBehaviour
    {
        var type = typeof(T).ToString();
        var prefab = await Resources.LoadAsync<T>($"UI/{type}") as T;
        var instance =  GameObject.Instantiate<T>(prefab, UICanvas.DefaultCanvas.transform, false);
        return instance;
    }
}
