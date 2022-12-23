using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AssetBuilder
{
    [MenuItem("Tools/AssetBuilder/BuildStreamingAsset")]
    public static AddressablesPlayerBuildResult BuildAssetToStreamingGroup()
    {
        Directory.GetFiles($"{Application.dataPath}/App/AssetBundles", "*.*", SearchOption.AllDirectories)
                 .Where(x => !x.EndsWith(".meta"))
                 .Where(IsNotInDefaultGroup)
                 .ToList()
                 .ForEach(ToStreamingGroup);
        return BuildContent();
    }

    [MenuItem("Tools/AssetBuilder/BuildRemoteBundle")]
    public static AddressablesPlayerBuildResult BuildAssetToRemoteGroup()
    {
        UpdateAssetToRemoteGroup();
        return BuildContent();
    }

    private static void UpdateAssetToRemoteGroup()
    {
        Directory.GetFiles(GetAssetBundlePath(), "*.*", SearchOption.AllDirectories)
                         .Where(x => !x.EndsWith(".meta"))
                         .Where(IsNotInDefaultGroup)
                         .ToList()
                         .ForEach(ToRemoteGroup);
    }

    [MenuItem("Tools/AssetBuilder/ClearAllCache")]
    public static void ClearAllCache()
    {
        Debug.Log("clear cache at " + Application.persistentDataPath);
        var list = Directory.GetDirectories(Application.persistentDataPath);

        foreach (var item in list)
        {
            Debug.Log("Delete" + " " + item);
            Directory.Delete(item, true);
        }

        Caching.ClearCache();
    }

    [MenuItem("Tools/AssetBuilder/UpdateAssetsAddress")]
    public static void UpdateAssetsAddress()
    {
        UpdateAssetToRemoteGroup();
        AssetDatabase.Refresh();
    }

    public static AddressablesPlayerBuildResult BuildContent()
    {
        AddressableAssetSettings.BuildPlayerContent(out var result);
        return result;
    }

    private static string GetAssetBundlePath()
    {
        return $"{Application.dataPath}/App/AssetBundles/";
    }

    private static void ToRemoteGroup(string file)
    {
        var fileWithOutExtension = file.Replace(Path.GetExtension(file), "").Replace("\\","/");
        string guid = GetGUID(file);
        var key = fileWithOutExtension.Substring(GetAssetBundlePath().Length);
        var group = AddressableAssetSettingsDefaultObject.Settings.groups.FirstOrDefault(x => key.Contains(x.Name));
        if (group != default)
        {
            var entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(guid, group);
            entry.SetAddress(key);
            AddLabelIfHorses(entry, Path.GetFileName(file));    
        }
        
    }

    private static void AddLabelIfHorses(AddressableAssetEntry entry, string label)
    {
        if (entry.parentGroup.Name == "Horses")
        {
            entry.SetLabel(AddressableAssetSettingsDefaultObject.Settings.GetLabels().FirstOrDefault(x => x == label), true);
        }
    }

    private static void ToStreamingGroup(string file)
    {
        string guid = GetGUID(file);
        var streamingGroup = AddressableAssetSettingsDefaultObject.Settings.groups.FirstOrDefault(x => x.Name == "Streaming");
        AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(guid, streamingGroup);
    }

    private static bool IsNotInDefaultGroup(string file)
    {
        string guid = GetGUID(file);
        var entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
        if (entry?.parentGroup.Name == "Default Local Group")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static string GetGUID(string file)
    {
        string relativepath = ToRelativePath(file);
        var guid = AssetDatabase.AssetPathToGUID(relativepath);
        return guid;
    }

    private static string ToRelativePath(string file)
    {
        return "Assets" + file.Substring(Application.dataPath.Length);
    }

    public static void AddAssetToGroup(string path, string groupName)
    {
        var group = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
        if (!group)
        {
            throw new Exception($"Addressable : can't find group {groupName}");
        }
        var entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group,
            false,
            true);

        if (entry == null)
        {
            throw new Exception($"Addressable : can't add {path} to group {groupName}");
        }
    }


}
