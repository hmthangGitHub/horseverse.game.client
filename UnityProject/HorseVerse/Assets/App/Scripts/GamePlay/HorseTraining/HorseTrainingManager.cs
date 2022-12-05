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

    public async UniTask Initialize(string mapPath, Action onTakeCoin, Action onTouchObstacle,
        MasterHorseTrainingProperty masterHorseTrainingProperty,
        MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
        MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
        HorseMeshInformation horseMeshInformation)
    {
        await horseTrainingController.Initialize(masterHorseTrainingProperty, horseMeshInformation);
        horseTrainingController.OnTakeCoin += onTakeCoin;
        horseTrainingController.OnDeadEvent += onTouchObstacle;
        await PlatformGenerator.InitializeAsync(masterHorseTrainingProperty, masterHorseTrainingBlockContainer, masterHorseTrainingBlockComboContainer);
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
