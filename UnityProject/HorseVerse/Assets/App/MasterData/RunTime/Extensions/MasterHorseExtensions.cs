using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MasterHorseExtensions
{
    public static MasterHorse FromTypeToMasterHorse(this MasterHorseContainer masterHorseContainer,
                                                    int type)
    {
        return masterHorseContainer.MasterHorseIndexer.Values.FirstOrDefault(x => (int)x.MasterHorseType == type);
    }
}
