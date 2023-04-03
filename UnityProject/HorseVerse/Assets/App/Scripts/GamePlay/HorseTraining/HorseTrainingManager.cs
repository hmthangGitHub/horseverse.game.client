using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class HorseTrainingManager : MonoBehaviour, IDisposable
{
    [SerializeField] private HorseTrainingControllerV2 horseTrainingController;
    [SerializeField] private PlatformGeneratorBase platformGenerator;

    public PlatformGeneratorBase PlatformGenerator => platformGenerator;

    public HorseTrainingControllerV2 HorseTrainingController => horseTrainingController;

    public async UniTask Initialize(string mapPath,
                                    string mapId,
                                    int NumberOfBlock,
                                    Action onTakeCoin,
                                    Action onUpdateRunTime,
                                    Action onTouchObstacle,
                                    Action onFinishOnePlatform,
                                    Action onFinishOneScene,
                                    MasterHorseTrainingProperty masterHorseTrainingProperty,
                                    MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                    MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
                                    MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer,
                                    MasterTrainingDifficultyContainer masterTrainingDifficultyContainer,
                                    HorseMeshInformation horseMeshInformation)
    {
        await HorseTrainingController.Initialize(masterHorseTrainingProperty, masterTrainingDifficultyContainer, horseMeshInformation);
        HorseTrainingController.OnTakeCoin += onTakeCoin;
        HorseTrainingController.OnUpdateRunTime += onUpdateRunTime;
        HorseTrainingController.OnDeadEvent += onTouchObstacle;
        PlatformGenerator.OnFinishOnePlatform += onFinishOnePlatform;
        PlatformGenerator.OnFinishOneScene += onFinishOneScene;
        await PlatformGenerator.InitializeAsync(masterHorseTrainingProperty, 
            masterHorseTrainingBlockContainer, 
            masterHorseTrainingBlockComboContainer, 
            masterTrainingDifficultyContainer, 
            masterTrainingBlockDistributeContainer,
            mapId,
            NumberOfBlock,
            Vector3.forward);
    }

    public void StartGame()
    {
        HorseTrainingController.IsStart = true;
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref horseTrainingController);
    }

    public async UniTask UpdateMap( string mapPath,
                                    string mapId,
                                    int NumberOfBlock,
                                    MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                    MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer)
    {
        var dir = PlatformGenerator.NextDirection;
        await PlatformGenerator.UpdateMapAsync(masterHorseTrainingBlockContainer,
            masterHorseTrainingBlockComboContainer,
            mapId,
            NumberOfBlock,
            dir);
    }

    public async UniTask PerformHighJumpToChangeSceneAsync()
    {
        await horseTrainingController.PerformHighJumpToChangeSceneAsync();
    }

    public void LandToNewScene()
    {
        horseTrainingController.LandToNewScene();
    }

    public async UniTask GenerateMultiBlockAsyncWhenChangeScene(int numberOfBlock)
    {
        await PlatformGenerator.GenerateMultiBlockAsyncWhenChangeScene(numberOfBlock);
    }
}
