using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class HorseTrainingManager : MonoBehaviour, IDisposable
{
    [SerializeField] private MapGenerator mapGenerator; // TODO load
    [SerializeField] private HorseTrainingControllerV2 horseTrainingController; // TODO load
    [SerializeField] private PlatformGenerator platformGenerator; // TODO load

    public PlatformGenerator PlatformGenerator => platformGenerator;

    public async UniTask Initialize(string mapPath, Action onTakeCoin, Action onTouchObstacle,
        MasterHorseTrainingProperty masterHorseTrainingProperty,
        MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
        MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
        HorseMeshAssetLoader.HorseMeshInformation horseMeshInformation)
    {
        await horseTrainingController.Initialize(masterHorseTrainingProperty, horseMeshInformation);
        horseTrainingController.OnTakeCoin += onTakeCoin;
        horseTrainingController.OnDeadEvent += onTouchObstacle;
        PlatformGenerator.SetMasterHorseTrainingProperty(masterHorseTrainingProperty, masterHorseTrainingBlockContainer, masterHorseTrainingBlockComboContainer);
    }

    public void StartGame()
    {
        horseTrainingController.IsStart = true;
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref horseTrainingController);
    }
}
