using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseRacePresenterTest : MonoBehaviour
{
    async void Start()
    {
        var container = new DIContainer();
        UserDataRepository userDataRepository = new UserDataRepository();
        await userDataRepository.LoadRepositoryIfNeedAsync();
        container.Bind(userDataRepository);
        container.Bind(await MasterLoader.LoadMasterAsync<MasterHorseContainer>());
        container.Bind(FindMatch(container));

        var horsePresenter = new HorseRacePresenter(container);
        await horsePresenter.LoadAssetAsync();
        await horsePresenter.PlayIntro();
        horsePresenter.StartGame();
    }

    public RaceMatchData FindMatch(DIContainer container)
    {
        int[] GetHorseTopPosition()
        {
            return Enumerable.Range(1, 8).Shuffle().ToArray();
        }

        long[] GetAllMasterHorseIds()
        {
            return container.Inject<MasterHorseContainer>().MasterHorseIndexer.Keys
                            .Shuffle()
                            .Append(container.Inject<UserDataRepository>().Current.MasterHorseId)
                            .Shuffle()
                            .Take(8)
                            .ToArray();
        }

        return new RaceMatchData()
        {
            masterHorseIds = GetAllMasterHorseIds(),
            tops = GetHorseTopPosition(),
            masterMapId = 10001002
        };
    }
}
