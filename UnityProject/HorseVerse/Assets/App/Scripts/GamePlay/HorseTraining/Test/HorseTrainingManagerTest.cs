using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var masterHorseTrainingPropertyContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingPropertyContainer>();
        var masterHorseTrainingBlockContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockContainer>();
        var masterHorseTrainingBlockComboContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockComboContainer>();
        
        await UniTask.DelayFrame(2);
        await horseTrainingManager.Initialize("", "", default, default, masterHorseTrainingPropertyContainer.DataList.First(), 
            masterHorseTrainingBlockContainer ,
            masterHorseTrainingBlockComboContainer);
    }
}
