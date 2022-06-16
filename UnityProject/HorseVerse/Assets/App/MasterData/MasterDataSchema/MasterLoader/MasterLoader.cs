using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MasterLoader
{
    public static async UniTask<TMasterContainer> LoadAsync<TMasterContainer>() where TMasterContainer : IMasterContainer, new()
    {
        var masterContainer = new TMasterContainer();
        var textAsset = await Resources.LoadAsync<TextAsset>($"MasterData/{masterContainer.MasterType}") as TextAsset;
        masterContainer.SetDataList(textAsset.text);
        return masterContainer;
    }
}
 