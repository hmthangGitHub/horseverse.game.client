using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class HorseTrainingManagerTest : MonoBehaviour
{
    public HorseTrainingManager horseTrainingManager;
    private HorseTrainingManager clonedHorseTrainingManager;
    
    private void OnGUI()
    {
        if (GUILayout.Button("Start"))
        {
            StartAsync().Forget();
        }
    }

    private async UniTaskVoid StartAsync()
    {
        horseTrainingManager.gameObject.SetActive(false);
        Destroy(clonedHorseTrainingManager?.gameObject);
        clonedHorseTrainingManager = Object.Instantiate(horseTrainingManager);
        clonedHorseTrainingManager.gameObject.SetActive(true);
        await UniTask.DelayFrame(2);
        await clonedHorseTrainingManager.Initialize("", "", default, default);
    }
}
