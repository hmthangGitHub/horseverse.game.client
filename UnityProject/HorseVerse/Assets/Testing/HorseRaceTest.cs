using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseRaceTest : MonoBehaviour
{
    public HorseRaceManager horseRaceManager;

    private void Start()
    {
        int[] horseIdInLanes = RandomHorseInLanes();
        horseRaceManager.StartRace(horseIdInLanes, UnityEngine.Random.Range(0, 7));
    }

    private static int[] RandomHorseInLanes()
    {
        var randomizeHorsePath = HorseMasterDataContainer.HorseModelPaths.Shuffle().ToList();
        var horseIdInLanes = HorseMasterDataContainer.HorseModelPaths.Select(x => randomizeHorsePath.FindIndex(path => path == x)).ToArray();
        return horseIdInLanes;
    }
}
