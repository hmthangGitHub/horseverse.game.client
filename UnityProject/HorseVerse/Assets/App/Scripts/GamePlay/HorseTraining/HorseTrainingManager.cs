using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HorseTrainingManager : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator; // TODO load
    [SerializeField] private HorseTrainingController horseTrainingController; // TODO load

    public async UniTask Initialize(string horseTrainingPath, string mapPath, Action onTakeCoin, Action onTouchObstacle)
    {
        await UniTask.CompletedTask;
    }
}
