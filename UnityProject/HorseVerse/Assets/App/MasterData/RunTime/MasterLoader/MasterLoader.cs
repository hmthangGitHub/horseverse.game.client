using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class MasterLoader
{
    public static async UniTask<TMasterContainer> LoadMasterAsync<TMasterContainer>(CancellationToken token = default) where TMasterContainer : IMasterContainer, new()
    {
        var masterContainer = new TMasterContainer();
        var text = await GetMasterRawDataText<TMasterContainer>(token);
#if ENABLE_DEBUG_MODULE
        try
        {
#endif
            masterContainer.SetDataList(text);
#if ENABLE_DEBUG_MODULE
        }
        catch
        {
            PlayerPrefs.DeleteKey(GetMasterPath<TMasterContainer>());
            throw;
        }
#endif
        return masterContainer;
    }

    public static async UniTask<TMasterContainer> LoadMasterAsync<TMasterContainer>(string prefix, CancellationToken token = default) where TMasterContainer : IMasterContainer, new()
    {
        var masterContainer = new TMasterContainer();
        var text = await GetMasterRawDataText<TMasterContainer>(prefix, token);
#if ENABLE_DEBUG_MODULE
        try
        {
#endif
            masterContainer.SetDataList(text);
#if ENABLE_DEBUG_MODULE
        }
        catch
        {
            PlayerPrefs.DeleteKey(GetMasterPath<TMasterContainer>());
            throw;
        }
#endif
        return masterContainer;
    }



    private static async UniTask<string> GetMasterRawDataText<TMasterContainer>(CancellationToken token) where TMasterContainer : IMasterContainer, new()
    {
        var masterPath = GetMasterPath<TMasterContainer>();
#if ENABLE_DEBUG_MODULE
        if(PlayerPrefs.HasKey(masterPath))
        {
            return PlayerPrefs.GetString(masterPath);
        }
#endif
        return (await PrimitiveAssetLoader.LoadAssetAsync<TextAsset>(masterPath, token)).text;
    }

    private static async UniTask<string> GetMasterRawDataText<TMasterContainer>(string prefix, CancellationToken token) where TMasterContainer : IMasterContainer, new()
    {
        var masterPath = $"{GetMasterPath<TMasterContainer>()}_{prefix}";
#if ENABLE_DEBUG_MODULE
        if (PlayerPrefs.HasKey(masterPath))
        {
            return PlayerPrefs.GetString(masterPath);
        }
#endif
        return (await PrimitiveAssetLoader.LoadAssetAsync<TextAsset>(masterPath, token)).text;
    }

    public static string GetMasterPath<TMasterContainer>() where TMasterContainer : IMasterContainer
    {
        return GetMasterPath(typeof(TMasterContainer));
    }

    public static string GetMasterPath(Type type)
    {
        return $"MasterData/{type}".Replace("Container", "");
    }

    public static void Unload<TMasterContainer>() where TMasterContainer : IMasterContainer
    {
#if ENABLE_DEBUG_MODULE
        if(PlayerPrefs.HasKey(GetMasterPath<TMasterContainer>()))
        {
            return;
        }
#endif
        PrimitiveAssetLoader.UnloadAssetAtPath(GetMasterPath<TMasterContainer>());
    }

    public static void Unload<TMasterContainer>(string prefix) where TMasterContainer : IMasterContainer
    {
        var masterPath = $"{GetMasterPath<TMasterContainer>()}_{prefix}";
#if ENABLE_DEBUG_MODULE
        if (PlayerPrefs.HasKey(masterPath))
        {
            return;
        }
#endif
        PrimitiveAssetLoader.UnloadAssetAtPath(masterPath);
    }

    public static void SafeRelease<TMasterContainer>(ref TMasterContainer masterContainer) where TMasterContainer : IMasterContainer
    {
        if (masterContainer != null)
        {
            masterContainer = default;
            MasterLoader.Unload<TMasterContainer>();
        }
    }

    public static void SafeRelease<TMasterContainer>(string prefix, ref TMasterContainer masterContainer) where TMasterContainer : IMasterContainer
    {
        if (masterContainer != null)
        {
            masterContainer = default;
            MasterLoader.Unload<TMasterContainer>(prefix);
        }
    }
}
 