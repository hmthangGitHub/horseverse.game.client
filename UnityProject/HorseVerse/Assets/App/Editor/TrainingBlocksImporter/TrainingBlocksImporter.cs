using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodiceApp.EventTracking.Plastic;
using ICSharpCode.NRefactory.Ast;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

public class TrainingBlocksImporter
{
    private const string TrainingBlocksPath = "Assets/App/Prefabs/GameMode/HorseTraining/MapSettings/trainingMapBlock/BlockPredefined/TrainingBlocks";
    private const string TrainingObstaclesPath = "Assets/App/Prefabs/GameMode/HorseTraining/MapSettings/trainingMapBlock/BlockPredefined/Obstacles";
    private const string TrainingSceneryObjectsPath = "Assets/App/Prefabs/GameMode/HorseTraining/MapSettings/trainingMapBlock/BlockPredefined/SceneryObjects";
    private const string TrainingBlockModularPath = "Assets/App/Prefabs/GameMode/HorseTraining/MapSettings/trainingMapBlock/BlockModular";
    private const string TrainingBlockSettings = "Assets/App/AssetBundles/Maps/MapSettings/training_block_settings.asset";

    [MenuItem("Assets/Importer/ImportBlocks", true)]
    public static bool ImportBlockCombosValidate()
    {
        return Selection.gameObjects.All(x => PrefabUtility.GetPrefabAssetType(x) == PrefabAssetType.Model
                                              && x.transform.Cast<Transform>().Any(child => IsFloor(child, x)) != default);
    }

    private static bool IsFloor(Transform child,
                                GameObject x)
    {
        return child.name.Contains(x.name) && !child.name.Contains("standie");
    }

    [MenuItem("Assets/Importer/ImportBlocks")]
    public static void ImportBlockCombos()
    {
        Selection.gameObjects.ForEach(ImportBlockCombo);
        SaveToBlockSettings();
    }

    private static void SaveToBlockSettings()
    {
        var trainingBlockSetting = AssetDatabase.LoadAssetAtPath<TrainingBlockSettings>(TrainingBlockSettings);
        trainingBlockSetting.blockCombos = LoadAllAssetAtPath<TrainingBlockPredefine>(TrainingBlocksPath)
            .Where(x => !x.name.Contains("starting_block"))
            .ToArray();
        EditorUtility.SetDirty(trainingBlockSetting);
    }

    public static T[] LoadAllAssetAtPath<T>(string folder) where T : UnityEngine.Object
    {
        return Directory.GetFiles(folder)
                 .Where(x => !x.EndsWith(".meta"))
                 .Select(x =>
                 {
                     try
                     {
                         return AssetDatabase.LoadAssetAtPath<T>(x);
                     }
                     catch (Exception e)
                     {
                         return default;
                     }
                 })
                 .Where(x => x != default)
                 .ToArray();
    }

    private static void ImportBlockCombo(GameObject originalBlock)
    {
        ImportAsPrefabVariant(originalBlock, blockInstance =>
        {
            var trainingBlockPredefine = InstantiateTrainingBlockPredefine(blockInstance);

            AssignLayerToChild(blockInstance, trainingBlockPredefine);
            var platformGo = blockInstance.transform.Cast<Transform>().First(x => IsFloor(x, blockInstance.gameObject)).gameObject;
            CreateSceneryContainers(platformGo, platformGo.transform.position, blockInstance,trainingBlockPredefine);
        }, $"{TrainingBlocksPath}/{originalBlock.name}.prefab");
    }

    private static void AssignLayerToChild(GameObject blockInstance,
                                           TrainingBlockPredefine trainingBlockPredefine)
    {
        foreach (Transform child in blockInstance.transform)
        {
            var childGameObject = child.gameObject;
            if (IsFloor(child, blockInstance))
            {
                childGameObject.layer = LayerMask.NameToLayer("TrainingPlatform");
                childGameObject.tag = "Platform";
            }
            else if (child.name == "obstacle_dummy")
            {
            }
            else if (child.name == "collider")
            {
                child.Cast<Transform>()
                     .ForEach(x =>
                     {
                         var meshRenderer = x.GetComponent<MeshRenderer>();
                         meshRenderer.enabled = false;
                         x.gameObject.AddComponent<MeshCollider>();
                         x.gameObject.layer = LayerMask.NameToLayer("TrainingObject");
                         x.tag = "Obstacle";
                     });
            }
            else if (child.name == "coinblock")
            {
                trainingBlockPredefine.coinPositions = child.Cast<Transform>().Select(x => x.localPosition).ToArray();
                child.gameObject.SetActive(false);
            }
            else
            {
                childGameObject.AddComponent<MeshCollider>();
                childGameObject.layer = LayerMask.NameToLayer("TrainingObject");
                childGameObject.tag = "Obstacle";
            }
        }
    }

    private static void CreateSceneryContainers(GameObject childGameObject,
                                                Vector3 position,
                                                GameObject blockInstance,
                                                TrainingBlockPredefine trainingBlockPredefine)
    {
        var originalBoxCollider = childGameObject.AddComponent<BoxCollider>();
        var sceneryContainerBoxCollider = CreateSceneryContainerBoxCollider(position, blockInstance, originalBoxCollider);
        var sceneryConflictRegionBoxCollider = CreatSceneryConflictRegionBoxCollider(position, blockInstance, originalBoxCollider);

        trainingBlockPredefine.sceneryContainer = sceneryContainerBoxCollider;
        trainingBlockPredefine.sceneryConflictRegion = sceneryConflictRegionBoxCollider;
    }

    private static BoxCollider CreateSceneryContainerBoxCollider(Vector3 position,
                                                                 GameObject blockInstance,
                                                                 BoxCollider originalBoxCollider)
    {
        var sceneryContainer = new GameObject("SceneryContainer")
        {
            transform =
            {
                position = position,
                parent = blockInstance.transform
            },
            layer = default
        };
        var sceneryContainerBoxCollider = sceneryContainer.AddComponent<BoxCollider>();
        sceneryContainerBoxCollider.center = originalBoxCollider.center;
        sceneryContainerBoxCollider.size
            = new Vector3(originalBoxCollider.size.z * 2, originalBoxCollider.size.z, originalBoxCollider.size.z);
        sceneryContainerBoxCollider.isTrigger = true;
        return sceneryContainerBoxCollider;
    }

    private static BoxCollider CreatSceneryConflictRegionBoxCollider(Vector3 position,
                                                                     GameObject blockInstance,
                                                                     BoxCollider originalBoxCollider)
    {
        var sceneryConflictRegion = new GameObject("SceneryConflictRegion")
        {
            transform =
            {
                position = position,
                parent = blockInstance.transform
            },
            layer = default
        };
        var sceneryConflictRegionBoxCollider = sceneryConflictRegion.AddComponent<BoxCollider>();
        sceneryConflictRegionBoxCollider.center = originalBoxCollider.center;
        sceneryConflictRegionBoxCollider.size = new Vector3(originalBoxCollider.size.x * 2, originalBoxCollider.size.z,
            originalBoxCollider.size.z);
        sceneryConflictRegionBoxCollider.isTrigger = true;
        return sceneryConflictRegionBoxCollider;
    }

    private static TrainingBlockPredefine InstantiateTrainingBlockPredefine(GameObject blockInstance)
    {
        var trainingBlockPredefine = blockInstance.AddComponent<TrainingBlockPredefine>();
        trainingBlockPredefine.obstacleDummies = blockInstance.transform.Cast<Transform>()
                                                              .FirstOrDefault(x => x.name == "obstacle_dummy")
                                                              ?.Cast<Transform>()
                                                              .Select(x => x.gameObject)
                                                              .ToArray() ?? Array.Empty<GameObject>();
        return trainingBlockPredefine;
    }

    private static void ImportAsPrefabVariant(GameObject originalPrefab,
                                              Action<GameObject> variantFactory, 
                                              string destinationPrefab)
    {
        
        var original = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
        try
        {
            variantFactory.Invoke(original);
            PrefabUtility.SaveAsPrefabAsset(original, destinationPrefab);
        }
        finally
        {
            Object.DestroyImmediate(original);
        }
    }

    [MenuItem("Assets/Importer/ImportObstacles", true)]
    public static bool ImportObstaclesValidate()
    {
        return Selection.gameObjects.All(x => PrefabUtility.GetPrefabAssetType(x) == PrefabAssetType.Model
                                              && x.transform.Cast<Transform>().All(child => child.name.Contains(x.name)));
    }
    
    [MenuItem("Assets/Importer/ImportObstacles")]
    public static void ImportObstacles()
    {
        Selection.gameObjects.ForEach(ImportObstacle);
        var trainingBlockSetting = AssetDatabase.LoadAssetAtPath<TrainingBlockSettings>(TrainingBlockSettings);
        trainingBlockSetting.obstacles = LoadAllAssetAtPath<GameObject>(TrainingObstaclesPath);
        EditorUtility.SetDirty(trainingBlockSetting);
    }

    private static void ImportObstacle(GameObject obstacle)
    {
        ImportAsPrefabVariant(obstacle, obstacleInstance =>
        {
            foreach (Transform child in obstacleInstance.transform)
            {
                var childGameObject = child.gameObject;
                if (!child.name.Contains("dummy"))
                {
                    childGameObject.AddComponent<MeshCollider>();
                    childGameObject.layer = LayerMask.NameToLayer("TrainingObject");
                    childGameObject.tag = "Obstacle";
                }
            }
        }, $"{TrainingObstaclesPath}/{obstacle.name}.prefab");
    }
    
    [MenuItem("Assets/Importer/SceneryObjects", true)]
    public static bool ImportSceneryObjectsValidate()
    {
        return Selection.gameObjects.All(x => PrefabUtility.GetPrefabAssetType(x) == PrefabAssetType.Model);
    }
    
    [MenuItem("Assets/Importer/SceneryObjects")]
    public static void ImportSceneryObjects()
    {
        Selection.gameObjects.ForEach(ImportSceneryObject);
        var trainingBlockSetting = AssetDatabase.LoadAssetAtPath<TrainingBlockSettings>(TrainingBlockSettings);
        trainingBlockSetting.sceneryObjects = LoadAllAssetAtPath<GameObject>(TrainingSceneryObjectsPath)
            .SelectMany(x => x.transform.Cast<Transform>().Select(child =>child.gameObject))
            .ToArray();
        EditorUtility.SetDirty(trainingBlockSetting);
    }

    private static void ImportSceneryObject(GameObject obj)
    {
        ImportAsPrefabVariant(obj, sceneryObject => sceneryObject.transform.Cast<Transform>()
                                                                 .ForEach(x => x.gameObject.AddComponent<SceneryObjectAnimation>()),
            $"{TrainingSceneryObjectsPath}/{obj.name}.prefab");
    }
    
    [MenuItem("Assets/Importer/ImportBlockModular", true)]
    public static bool ImportBlockModularValidate()
    {
        return Selection.gameObjects.All(x =>
        {
            var children = x.transform.Cast<Transform>().ToArray();
            return children.Any(child => child.name == "platform" )
                && children.Any(child => child.name == "standie");
        });
    }
    
    [MenuItem("Assets/Importer/ImportBlockModular")]
    public static void ImportBlocksModular()
    {
        Selection.gameObjects.ForEach(ImportBlockModular);
        var trainingBlockSetting = AssetDatabase.LoadAssetAtPath<TrainingBlockSettings>(TrainingBlockSettings);
        trainingBlockSetting.blocks = LoadAllAssetAtPath<GameObject>(TrainingBlockModularPath);
        EditorUtility.SetDirty(trainingBlockSetting);
    }

    private static void ImportBlockModular(GameObject obj)
    {
        ImportAsPrefabVariant(obj, blockModular =>
            {
                var children = blockModular.transform.Cast<Transform>().ToArray();
                children.First(x => x.name == "platform")
                      .gameObject.AddComponent<BoxCollider>();
                children.First(x => x.name == "standie")
                      .gameObject.AddComponent<MeshCollider>();
            },
            $"{TrainingBlockModularPath}/{obj.name}.prefab");
    }
}
