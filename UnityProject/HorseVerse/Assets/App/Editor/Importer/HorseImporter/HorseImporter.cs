using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class HorseImporter
{
    public static string horsePath = "Assets/App/AssetBundles/Horses/";
    public static string horseRaceModePath = "Assets/App/AssetBundles/Horses/RaceMode";
    public static string horseIntroPath = "Assets/App/AssetBundles/Horses/Intro";
    public static string horseControllerBasePath = "Assets/App/Prefabs/GameMode/HorseRacing/HorseControllerBase.prefab";

    [MenuItem("Assets/ImportHorse", true)]
    public static bool ImportHorseValidate()
    {
        return Selection.gameObjects.All(x => PrefabUtility.GetPrefabAssetType(x) == PrefabAssetType.Regular);
    }

    [MenuItem("Assets/ImportHorse")]
    public static void ImportHorse()
    {
        Selection.gameObjects.ForEach(x =>
        {
            var (path, name) = ImportHorseModel(x);
            ImportHorseRaceMode(path, name); 
            ImportHorseIntro(path, name); 
        });
        
    }

    private static void ImportHorseIntro(string horseModelPath, string name)
    {
        var horseModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseModelPath));
        horseModel.transform.localPosition = Vector3.zero;
        PrefabUtility.SaveAsPrefabAsset(horseModel, $"{horseIntroPath}/{name}.prefab");
        GameObject.DestroyImmediate(horseModel);
    }

    public static void ImportHorseRaceMode(string horseModelPath, string name)
    {
        var objSource = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseControllerBasePath)) as GameObject;
        var meshContainer = objSource.FindGameObjectByName("Mesh");
        var horseModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseModelPath), meshContainer.transform);
        horseModel.transform.localPosition = Vector3.zero;
        PrefabUtility.SaveAsPrefabAsset(objSource, $"{horseRaceModePath}/{name}.prefab");
        Object.DestroyImmediate(objSource);
    }

    private static (string path, string name) ImportHorseModel(GameObject prefab)
    {
        var objSource = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        objSource.transform.localPosition = Vector3.zero;
        string horseModelPath = $"{horsePath}/{prefab.name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(objSource, horseModelPath);
        Object.DestroyImmediate(objSource);
        AddressableAssetSettingsDefaultObject.Settings.AddLabel(prefab.name);
        return (horseModelPath, prefab.name);
    }
}
