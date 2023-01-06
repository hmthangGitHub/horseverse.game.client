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
        var matchData = await new LocalQuickRaceDomainService(container).FindMatch(0);
        container.Bind(matchData);
        var horsePresenter = new HorseRacePresenter(container);
        await horsePresenter.LoadAssetAsync();
        await horsePresenter.PlayIntro();
        horsePresenter.StartGame();
    }

    public RaceMatchData FindMatch(DIContainer container)
    {
        HorseRaceInfo[] GetAllMasterHorseIds()
        {
            return container.Inject<MasterHorseContainer>().MasterHorseIndexer.Keys
                            .Shuffle()
                            .Append(container.Inject<UserDataRepository>().Current.CurrentHorseNftId)
                            .Shuffle()
                            .Take(8)
                            .Select(x => new HorseRaceInfo()
                            {
                                // masterHorseId = x,
                            })
                            .ToArray();
        }

        return new RaceMatchData()
        {
            HorseRaceInfos = GetAllMasterHorseIds(),
            MasterMapId = QuickRaceState.MasterMapId,
            Mode = RaceMode.Race
        };
    }
}
