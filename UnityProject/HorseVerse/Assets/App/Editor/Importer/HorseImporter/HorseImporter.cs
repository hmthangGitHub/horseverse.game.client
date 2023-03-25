using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class HorseImporter
{
    private static string horsePath = "Assets/App/AssetBundles/Horses/";
    private static string horseRaceModePath = "Assets/App/AssetBundles/Horses/RaceMode";
    private static string horseRaceThirdPersonModePath = "Assets/App/AssetBundles/Horses/RaceModeThirdPerson";
    private static string horseIntroPath = "Assets/App/AssetBundles/Horses/Intro";
    
    private static string horseControllerBasePath = "Assets/App/Prefabs/GameMode/HorseRacing/HorseControllerBase.prefab";
    private static string horseRaceThirdPersonBehaviourBasePath = "Assets/App/Prefabs/GameMode/HorseRaceThirdPerson/HorseRaceThirdPersonBehaviourBase.prefab";

    [MenuItem("Assets/Importer/ImportHorse", true)]
    public static bool ImportHorseValidate()
    {
        return Selection.gameObjects.All(x => PrefabUtility.GetPrefabAssetType(x) == PrefabAssetType.Regular);
    }

    [MenuItem("Assets/Importer/ImportHorse")]
    public static void ImportHorse()
    {
        Selection.gameObjects.ForEach(x =>
        {
            var (path, name) = ImportHorseModel(x);
            ImportHorseRaceMode(path, name); 
            ImportHorseIntro(path, name);
            ImportHorseRaceThirdPerson(path, name);
        });
        
    }

    private static void ImportHorseRaceThirdPerson(string path,
                                                  string name)
    {
        horseRaceThirdPersonBehaviourBasePath.ImportAsPrefabVariant(prefabVariant =>
        {
            var horseModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            horseModel.transform.parent = prefabVariant.GetComponent<HorseRaceThirdPersonBehaviour>().horseMeshContainer;
            horseModel.transform.localPosition = Vector3.zero;
            horseModel.transform.localRotation = Quaternion.identity;
            prefabVariant.GetComponentInChildren<HorseRaceFirstPersonAnimatorController>(true).Reset();
        }, $"{horseRaceThirdPersonModePath}/{name}.prefab");
    }

    private static void ImportHorseIntro(string horseModelPath, string name)
    {
        var horseModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseModelPath));
        horseModel.transform.localPosition = Vector3.zero;
        PrefabUtility.SaveAsPrefabAsset(horseModel, $"{horseIntroPath}/{name}.prefab");
        GameObject.DestroyImmediate(horseModel);
    }

    private static void ImportHorseRaceMode(string horseModelPath, string name)
    {
        horseControllerBasePath.ImportAsPrefabVariant(prefabVariant =>
        {
            var meshContainer = prefabVariant.FindGameObjectByName("Mesh");
            var horseModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(horseModelPath), meshContainer.transform);
            horseModel.transform.localPosition = Vector3.zero;
        }, $"{horseRaceModePath}/{name}.prefab");
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
