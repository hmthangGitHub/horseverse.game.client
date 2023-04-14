using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterTrainingBlockDistribute
{
#if ENABLE_DEBUG_MODULE
    public MasterTrainingBlockDistribute(long masterTrainingBlockDistributeID, int _difficult = default, int masterHorseTrainingBlockGroupID = default, int _weight = default)
    {
        master_training_block_distribute_id = masterTrainingBlockDistributeID;
        difficulty = _difficult;
        master_horse_training_block_group_id = masterHorseTrainingBlockGroupID;
        weight = _weight;
    }

    public void Set(long masterTrainingBlockDistributeID, int _difficult, int masterHorseTrainingBlockGroupID, int _weight)
    {
        master_training_block_distribute_id = masterTrainingBlockDistributeID;
        difficulty = _difficult;
        master_horse_training_block_group_id = masterHorseTrainingBlockGroupID;
        weight = _weight;
    }
#endif
}
