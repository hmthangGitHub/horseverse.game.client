using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public static class HorseSpriteAssetLoader
{
    public static async UniTask<Sprite> InstantiateHorseAvatar(string path, CancellationToken token = default)
    {
        var tex = await PrimitiveAssetLoader.LoadAssetAsync<Texture2D>(path, token);
        if (tex != default)
        {
            return ImageHelper.CreateSprite(tex); 
        }
        return null;
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
