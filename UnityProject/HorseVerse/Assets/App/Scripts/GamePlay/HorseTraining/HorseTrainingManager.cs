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
    [SerializeField] private PlatformGeneratorTest platformGeneratorTest; // TODO load

    public async UniTask Initialize(string horseTrainingPath, string mapPath, Action onTakeCoin, Action onTouchObstacle, MasterHorseTrainingProperty masterHorseTrainingProperty)
    {
        await UniTask.CompletedTask;
        horseTrainingController.OnTakeCoin += onTakeCoin;
        horseTrainingController.OnDeadEvent += onTouchObstacle;
        horseTrainingController.SetMasterHorseTrainingProperty(masterHorseTrainingProperty);
        platformGeneratorTest.SetMasterHorseTrainingProperty(masterHorseTrainingProperty);
    }

    public void StartGame()
    {
        horseTrainingController.IsStart = true;
    }
}
