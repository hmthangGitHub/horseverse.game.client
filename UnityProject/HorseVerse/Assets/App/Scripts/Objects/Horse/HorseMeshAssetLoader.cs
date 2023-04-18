using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class HorseMeshAssetLoader
{
    public static UniTask<GameObject> InstantiateHorse(HorseMeshInformation horseMeshInformation, CancellationToken token = default)
    {
        return InstantiateHorse(horseMeshInformation.horseModelPath, token);
    }
    
    public static async UniTask<GameObject> InstantiateHorse(string path, Color c1, Color c2, Color c3, Color c4, CancellationToken token = default)
    {
        var horsePrefab = await PrimitiveAssetLoader.LoadAssetAsync<GameObject>(path, token);
        var horse = Object.Instantiate(horsePrefab);
        horse.GetComponentInChildren<HorseObjectData>().SetColor(c1, c2, c3, c4);
        return horse;
    }

    public static async UniTask<GameObject> InstantiateHorse(string path, CancellationToken token = default)
    {
        var horsePrefab = await PrimitiveAssetLoader.LoadAssetAsync<GameObject>(path, token);
        var horse = Object.Instantiate(horsePrefab);
        return horse;
    }

    public static void SafeRelease(ref GameObject horse, string assetPath)
    {
        if (horse != default)
        {
            Object.Destroy(horse);
            horse = default;
            PrimitiveAssetLoader.UnloadAssetAtPath(assetPath);
        }
    }
}
