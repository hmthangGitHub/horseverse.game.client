using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MasterLoader
{
    public static async UniTask<TMasterContainer> LoadMasterAsync<TMasterContainer>() where TMasterContainer : IMasterContainer, new()
    {
        var masterContainer = new TMasterContainer();
        var textAsset = await PrimitiveAssetLoader.LoadAsset<TextAsset>(GetMasterPath<TMasterContainer>());
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
 