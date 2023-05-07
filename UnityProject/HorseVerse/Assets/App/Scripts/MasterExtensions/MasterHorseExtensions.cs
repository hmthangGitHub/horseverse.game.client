using System.Collections;
using System.Collections.Generic;
using System.Linq;
using io.hverse.game.protogen;
using UnityEngine;

public static class MasterHorseExtensions
{
    public static MasterHorse FromTypeToMasterHorse(this MasterHorseContainer masterHorseContainer,
                                                    HorseType type)
    {
        var masterHorseType = (MasterHorseType)type;
        return masterHorseContainer.MasterHorseIndexer.Values.FirstOrDefault(x => x.MasterHorseType == masterHorseType);
    }

    public static MasterHorse FromColorToMasterHorse(this MasterHorseContainer masterHorseContainer,
                                                    int color)
    {
        var id = 10000000 + color;
        if (color == 0) id = 10000001;
        return masterHorseContainer.MasterHorseIndexer.Values.FirstOrDefault(x => x.MasterHorseId == id);
    }
}
