using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public static class MasterLoader
{
    public static async UniTask<TMasterContainer> LoadMasterAsync<TMasterContainer>(CancellationToken token = default) where TMasterContainer : IMasterContainer, new()
    {
        var masterContainer = new TMasterContainer();
        var textAsset = await PrimitiveAssetLoader.LoadAsset<TextAsset>(GetMasterPath<TMasterContainer>(), token);
        masterContainer.SetDataList(textAsset.text);
        return masterContainer;
    }

    private static string GetMasterPath<TMasterContainer>() where TMasterContainer : IMasterContainer, new()
    {
        return $"MasterData/{typeof(TMasterContainer)}".Replace("Container", "");
    }

    public static void Unload<TMasterContainer>() where TMasterContainer : IMasterContainer, new()
    {
        PrimitiveAssetLoader.UnloadAssetAtPath(GetMasterPath<TMasterContainer>());
    }
}
 