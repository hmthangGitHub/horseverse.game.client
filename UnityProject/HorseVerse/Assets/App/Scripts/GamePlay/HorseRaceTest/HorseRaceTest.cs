using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HorseRaceTest : MonoBehaviour
{
    public HorseRaceManager horseRaceManager;
    public long masterMapId = 10001002;

    private async void Start()
    {
        var masterHorseContainer = await MasterLoader.LoadMasterAsync<MasterHorseContainer>();
        var masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>();
        long[] horseIdInLanes = RandomMasterHorseIdsInLanes(masterHorseContainer);

        await horseRaceManager.InitializeAsync(horseIdInLanes.Select(x => masterHorseContainer.MasterHorseIndexer[x].RaceModeModelPath).ToArray(),
                                               masterMapContainer.MasterMapIndexer[masterMapId].MapSettings,
                                               UnityEngine.Random.Range(0, 7),
                                               Enumerable.Range(1, 8).Shuffle().ToArray(),
                                               default);
        horseRaceManager.StartRace();
    }

    private static long[] RandomMasterHorseIdsInLanes(MasterHorseContainer masterHorseContainer)
    {
        var maxHorseInLane = 8;
        return masterHorseContainer.MasterHorseIndexer.Keys.Shuffle()
                                                           .Take(maxHorseInLane)
                                                           .ToArray();
        //var randomizeHorsePath = HorseMasterDataContainer.HorseModelPaths.Shuffle().ToList();
        //var horseIdInLanes = HorseMasterDataContainer.HorseModelPaths.Select(x => randomizeHorsePath.FindIndex(path => path == x)).ToArray();
        //return horseIdInLanes;
    }
}
