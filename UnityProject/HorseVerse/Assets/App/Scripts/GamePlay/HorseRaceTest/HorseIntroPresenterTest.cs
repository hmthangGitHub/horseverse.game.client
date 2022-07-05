using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseIntroPresenterTest : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        var container = new DIContainer();
        MasterHorseContainer masterHorseContaienr = await MasterLoader.LoadMasterAsync<MasterHorseContainer>();
        container.Bind(masterHorseContaienr);
        var horseIntroPresenter = new RaceModeHorseIntroPresenter(container);
        await horseIntroPresenter.ShowHorsesInfoIntroAsync(GetAllMasterHorseIds(masterHorseContaienr), Vector3.zero, Quaternion.identity);
    }

    private long[] GetAllMasterHorseIds(MasterHorseContainer horseContainer)
    {
        return horseContainer.MasterHorseIndexer.Keys
                        .Shuffle()
                        .Take(8)
                        .ToArray();
    }
}
