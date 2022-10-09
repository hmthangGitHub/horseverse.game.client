using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseTrainingPresenterTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var di = new DIContainer();
        di.Bind(new HorseTrainingDataContext()
        {
            masterHorseId = 10000001,
            masterMapId = 10001002,
        });
        var presenter = new HorseTrainingPresenter(di);
        await presenter.LoadAssetsAsync();
        await presenter.StartTrainingAsync();
    }
}
