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
            HorseMeshInformation = new HorseMeshInformation()
            {
                horseModelPath = "Horses/Thunder",
                color1 = Color.gray,
                color2 = Color.blue,
                color3 = Color.cyan,
                color4 = Color.magenta
            },
            MasterMapId = 10001003,
        });
        var presenter = new HorseTrainingPresenter(di);
        await presenter.LoadAssetsAsync();
        await presenter.StartTrainingAsync();
    }
}
