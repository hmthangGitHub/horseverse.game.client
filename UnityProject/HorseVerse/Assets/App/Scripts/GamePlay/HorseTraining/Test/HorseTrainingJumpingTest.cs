using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HorseTrainingJumpingTest : MonoBehaviour
{
    public HorseTrainingManager horseTrainingManager;

    private async void Start()
    {
        this.enabled = false;
        var masterHorseTrainingPropertyContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingPropertyContainer>();
        await UniTask.DelayFrame(2);
        await horseTrainingManager.Initialize("", "", default, default, masterHorseTrainingPropertyContainer.DataList.First());
        await UniTask.Delay(2000);
        this.enabled = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            horseTrainingManager.StartGame();
            this.enabled = false;
        }
    }
}
