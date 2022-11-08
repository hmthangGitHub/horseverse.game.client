using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class HorseTrainingManager : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator; // TODO load
    [SerializeField] private HorseTrainingControllerV2 horseTrainingController; // TODO load
    [SerializeField] private PlatformGenerator platformGenerator; // TODO load

    public PlatformGenerator PlatformGenerator => platformGenerator;

    public async UniTask Initialize(string horseTrainingPath, string mapPath, Action onTakeCoin, Action onTouchObstacle, 
        MasterHorseTrainingProperty masterHorseTrainingProperty , 
        MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
        MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer)
    {
        await UniTask.CompletedTask;
        horseTrainingController.OnTakeCoin += onTakeCoin;
        horseTrainingController.OnDeadEvent += onTouchObstacle;
        horseTrainingController.SetMasterHorseTrainingProperty(masterHorseTrainingProperty);
        PlatformGenerator.SetMasterHorseTrainingProperty(masterHorseTrainingProperty, masterHorseTrainingBlockContainer, masterHorseTrainingBlockComboContainer);
    }

    public void StartGame()
    {
        horseTrainingController.IsStart = true;
    }
}
