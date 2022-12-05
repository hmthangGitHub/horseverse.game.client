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
        using var horseIntroPresenter = new RaceModeHorseIntroPresenter(container);
        await horseIntroPresenter.LoadUIAsync();
        await horseIntroPresenter.ShowHorsesInfoIntroAsync(default, Vector3.zero, Quaternion.identity);
    }

    private long[] GetAllMasterHorseIds(MasterHorseContainer horseContainer)
    {
        return horseContainer.MasterHorseIndexer.Keys
                        .Shuffle()
                        .Take(9)
                        .ToArray();
    }
}
