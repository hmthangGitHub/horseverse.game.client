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
    public static string horseControllerBasePath = "Assets/App/Prefabs/HorseController/HorseControllerBase.prefab";

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
        });
        
    }

    public static void ImportHorseRaceMode(string horseModelPath, string name)
    {
        GameObject objSource = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseControllerBasePath)) as GameObject;
        var meshGO = objSource.FindGameObjectByName("Mesh");
        var horseModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseModelPath), meshGO.transform);
        horseModel.transform.localPosition = Vector3.zero;
        PrefabUtility.SaveAsPrefabAsset(objSource, $"{horseRaceModePath}/{name}.prefab");
        GameObject.DestroyImmediate(objSource);
    }

    private static (string path, string name) ImportHorseModel(GameObject prefab)
    {
        GameObject objSource = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        objSource.transform.localPosition = Vector3.zero;
        string horseModelPath = $"{horsePath}/{prefab.name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(objSource, horseModelPath);
        GameObject.DestroyImmediate(objSource);
        AddressableAssetSettingsDefaultObject.Settings.AddLabel(prefab.name);
        return (horseModelPath, prefab.name);
    }
}
