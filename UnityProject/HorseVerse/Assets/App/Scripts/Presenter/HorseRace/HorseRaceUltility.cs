using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HorseRaceUltility
{
    public static int[] GetTopByTimes(this float[] times)
    {
        return times.Select((x, i) => (x, i))
            .OrderBy(x => x.x)
            .Select(x => x.i + 1)
            .ToArray();
    }

    public static int[] GetTops(this RaceMatchData raceMatchData)
    {
        return raceMatchData.horseRaceTimes.Select(x => x.time).ToArray().GetTopByTimes();
    }
}
