using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public partial class LevelEditorPresenter
{
    private readonly List<(int index, GameObject obstacle)> obstacleInBlocks = new List<(int index, GameObject obstacle)>();
    private bool isEditingObstacle;
    private readonly List<GameObject> obstaclePinList = new List<GameObject>();

    public bool IsEditingObstacle
    {
        get => isEditingObstacle;
        set
        {
            if (isEditingObstacle == value) return;
            isEditingObstacle = value;
            OnChangeEditingObstacle();
        }
    }

    private void OnChangeEditingObstacle()
    {
        if (IsEditingObstacle)
        {
            GenerateObstacle().Forget();
        }
        else
        {
            SaveObstacleToBlockAndRemove();
        }
        
        uiDebugLevelEditor.isAddObstacleBtnVisible.SetEntity(IsEditingObstacle);
    }

    private void SaveObstacleToBlockAndRemove()
    {
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.ObstacleList = obstacleInBlocks.Select(x =>
                new Obstacle()
                {
                    type = trainingBlockSettings.obstacles[x.index]
                                                .name,
                    localPosition = Position.FromVector3(x.obstacle.transform.localPosition)
                })
            .ToArray();
        
        obstacleInBlocks.ForEach(x => Object.Destroy(x.obstacle));
        obstacleInBlocks.Clear();
        obstaclePinList.ForEach(x => Object.Destroy(x.gameObject));
        obstaclePinList.Clear();
    }
    
    private async UniTaskVoid GenerateObstacle()
    {
        await UniTask.Yield();
        var obstacles = trainingBlockSettings.obstacles.ToList();
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.ObstacleList
                                  .ForEach(x =>
                                  {
                                      CreateObstacleAtPosition(
                                          obstacles.FindIndex(saveObstacles => saveObstacles.name == x.type),
                                          x.localPosition.ToVector3()); 
                                  });
    }
    
    private void CreateNewObstacle()
    {
        var obstacleDummy = CreatObstacle(0);
        AddPinToObstacle(obstacleDummy);
    }
    
    private void CreateObstacleAtPosition(int index, Vector3 localPosition)
    {
        var obstacle = CreatObstacle(index);
        obstacle.transform.localPosition = new Vector3(localPosition.x, obstacle.transform.localPosition.y, localPosition.z);
        AddPinToObstacle(obstacle);
    }

    private GameObject CreatObstacle(int index)
    {
        var currentPlatformObject = (PlatformModular)currentEditingPlatformObject;
        var prefab = trainingBlockSettings.obstacles[index]
                             .transform.Cast<Transform>()
                             .First(x => x.gameObject.name.Contains("dummy"));
        var obstacleDummy = UnityEngine.Object.Instantiate(prefab, currentPlatformObject.transform).gameObject;
        obstacleDummy.name = prefab.name;
        var runtimeTransformHandle = obstacleDummy.AddComponent<RuntimeTransformHandle>();
        runtimeTransformHandle.axes = HandleAxes.XZ;
        
        PlatformModular.Snap(currentPlatformObject.FirstCollider, obstacleDummy.GetComponent<Collider>());
        obstacleInBlocks.Add((index, obstacleDummy));
        
        return obstacleDummy;
    }

    private void AddPinToObstacle(GameObject obstacle)
    {
        var pin = CreateUiPin(obstaclePinList);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            isDeleteBtnVisible = true,
            deleteBtn = new ButtonComponent.Entity(() =>
            {
                Object.Destroy(obstacle);
                RemovePin(pin);
                obstacleInBlocks.RemoveAll(x => x.obstacle == obstacle);
            }),
            shuffleBtn = new ButtonComponent.Entity(() =>
            {
                RemovePin(pin);
                Object.Destroy(obstacle);
                obstacleInBlocks.RemoveAll(x => x.obstacle == obstacle);
                ChangeObstacle(obstacle.name, obstacle.transform.localPosition);
            }),
            isShuffleBtnVisible = true,
            pinTransform = LevelDesignPin.Instantiate(obstacle.GetComponent<Collider>()).transform,
            camera = freeCameraComponent
        });
        pin.In().Forget();
    }

    private void ChangeObstacle(string obstacleName,
                                Vector3 localPosition)
    {
        var obstacleIndex = trainingBlockSettings.obstacles.ToList()
                                                 .FindIndex(x => x.transform.Cast<Transform>()
                                                                  .Any(child => child.gameObject.name == obstacleName));
        obstacleIndex = (obstacleIndex + 1) % trainingBlockSettings.obstacles.Length;
        CreateObstacleAtPosition(obstacleIndex, localPosition);
    }
}