using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class HorseImporter
{
    public static string horsePath = "Assets/App/AssetBundles/Horses/";
    public static string horseRaceModePath = "Assets/App/AssetBundles/Horses/RaceMode";
    public static string horseControllerBasePath = "Assets/App/Prefabs/HorseController/HorseControllerBase.prefab";

    [MenuItem("Assets/ImportHorse", true)]
    public static bool ImportHorseValidate()
    {
        return PrefabUtility.GetPrefabAssetType(Selection.activeObject) == PrefabAssetType.Regular;
    }

    [MenuItem("Assets/ImportHorse")]
    public static void ImportHorse()
    {
        var horseModelPath = ImportHorseModel();
        ImportHorseRaceMode(horseModelPath);
    }

    public static void ImportHorseRaceMode(string horseModelPath)
    {
        GameObject objSource = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseControllerBasePath)) as GameObject;
        var meshGO = objSource.FindGameObjectByName("Mesh");
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseModelPath), meshGO.transform);
        PrefabUtility.SaveAsPrefabAsset(objSource, $"{horseRaceModePath}/{Selection.activeObject.name}.prefab");
        GameObject.DestroyImmediate(objSource);
    }

    private static string ImportHorseModel()
    {
        GameObject objSource = PrefabUtility.InstantiatePrefab(Selection.activeObject) as GameObject;
        string horseModelPath = $"{horsePath}/{Selection.activeObject.name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(objSource, horseModelPath);
        GameObject.DestroyImmediate(objSource);
        AddressableAssetSettingsDefaultObject.Settings.AddLabel(Selection.activeObject.name);
        return horseModelPath;
    }
}
