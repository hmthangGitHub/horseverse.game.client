using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MasterTrainingBlockComboExtensions
{
    public static string GetFirstPaddingIfEmpty(this MasterTrainingModularBlockContainer masterTrainingModularBlockContainer, string masterTrainingModularBlockId)
    {
        return !string.IsNullOrEmpty(masterTrainingModularBlockId) 
            ? masterTrainingModularBlockId
            : masterTrainingModularBlockContainer.MasterTrainingModularBlockIndexer.Values
                                                 .First(x => x.MasterTrainingModularBlockType == MasterTrainingModularBlockType.Padding)
                                                 .MasterTrainingModularBlockId;
    }
}
