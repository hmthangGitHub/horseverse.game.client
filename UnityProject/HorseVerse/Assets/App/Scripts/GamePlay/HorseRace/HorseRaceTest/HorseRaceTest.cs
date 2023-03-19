using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HorseRaceTest : MonoBehaviour
{
    public HorseRaceManager horseRaceManager;
    public long masterMapId = RacingState.MasterMapId;

    private async void Start()
    {
        var masterHorseContainer = await MasterLoader.LoadMasterAsync<MasterHorseContainer>();
        var masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>();
        var horseIdInLanes = RandomMasterHorseIdsInLanes(masterHorseContainer);

        await horseRaceManager.InitializeAsync(default,
                                               masterMapContainer.MasterMapIndexer[masterMapId].MapSettings, 
                                    UnityEngine.Random.Range(0, 7),
                                               Enumerable.Range(1, 8).Select(x => UnityEngine.Random.Range(49.5f, 50.5f)).ToArray(),
                                               default,
                                               default);

        await horseRaceManager.WaitToStart();
        horseRaceManager.StartRace();
    }

    private static long[] RandomMasterHorseIdsInLanes(MasterHorseContainer masterHorseContainer)
    {
        var maxHorseInLane = 8;
        return masterHorseContainer.MasterHorseIndexer.Keys.Shuffle()
                                                           .Take(maxHorseInLane)
                                                           .ToArray();
    }
}
