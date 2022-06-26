using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class AdressableBuilder
{
    [MenuItem("Tool/AssetBuilder/BuildStreamingAsset")]
    public static void AddAssetToStreamingGroup()
    {
        Directory.GetFiles($"{Application.dataPath}/App/AssetBundles", "*.*", SearchOption.AllDirectories)
                 .Where(x => !x.EndsWith(".meta"))
                 .Where(IsNotInDefaultGroup)
                 .ToList()
                 .ForEach(ToStreamingGroup);
    }

    [MenuItem("Tool/AssetBuilder/BuildRemoteBundle")]
    public static void AddAssetToRemoteGroup()
    {
        Directory.GetFiles(GetAssetBundlePath(), "*.*", SearchOption.AllDirectories)
                 .Where(x => !x.EndsWith(".meta"))
                 .Where(IsNotInDefaultGroup)
                 .ToList()
                 .ForEach(ToRemoteGroup);
    }

    private static string GetAssetBundlePath()
    {
        return $"{Application.dataPath}/App/AssetBundles";
    }

    private static void ToRemoteGroup(string file)
    {
        string guid = GetGUID(file);
        var key = file.Substring(GetAssetBundlePath().Length);
        var group = AddressableAssetSettingsDefaultObject.Settings.groups.FirstOrDefault(x => key.Contains(x.Name));
        AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(guid, group);
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
