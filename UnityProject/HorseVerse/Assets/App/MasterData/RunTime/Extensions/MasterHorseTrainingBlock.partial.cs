#if ENABLE_MASTER_RUN_TIME_EDIT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterHorseTrainingBlock
{
    public MasterHorseTrainingBlock(long masterHorseTrainingBlockID, string name, MasterHorseTrainingLaneType masterHorseTrainingLaneTypeLeft = default, int customValueLeft = default, MasterHorseTrainingLaneType masterHorseTrainingLaneTypeMid = default, int customValueMid = default, MasterHorseTrainingLaneType masterHorseTrainingLaneTypeRight = default, int customValueRight = default)
    {
        master_horse_training_block_id = masterHorseTrainingBlockID;
        this.name = name;
        master_horse_training_lane_type_left = masterHorseTrainingLaneTypeLeft;
        custom_value_left = customValueLeft;
        master_horse_training_lane_type_mid = masterHorseTrainingLaneTypeMid;
        custom_value_mid = customValueMid;
        master_horse_training_lane_type_right = masterHorseTrainingLaneTypeRight;
        custom_value_right = customValueRight;
    }
    
    public void Set(long masterHorseTrainingBlockID, string name, MasterHorseTrainingLaneType masterHorseTrainingLaneTypeLeft, int customValueLeft, MasterHorseTrainingLaneType masterHorseTrainingLaneTypeMid, int customValueMid, MasterHorseTrainingLaneType masterHorseTrainingLaneTypeRight, int customValueRight)
    {
        master_horse_training_block_id = masterHorseTrainingBlockID;
        this.name = name;
        master_horse_training_lane_type_left = masterHorseTrainingLaneTypeLeft;
        custom_value_left = customValueLeft;
        master_horse_training_lane_type_mid = masterHorseTrainingLaneTypeMid;
        custom_value_mid = customValueMid;
        master_horse_training_lane_type_right = masterHorseTrainingLaneTypeRight;
        custom_value_right = customValueRight;
    }
    
    public (MasterHorseTrainingLaneType laneType, int customValue) this[int index]
    {
        get
        {
            return index switch
            {
                0 => (MasterHorseTrainingLaneTypeLeft, CustomValueLeft),
                1 => (MasterHorseTrainingLaneTypeMid, CustomValueMid),
                2 => (MasterHorseTrainingLaneTypeRight, CustomValueRight),
                _ => default
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    master_horse_training_lane_type_left = value.laneType;
                    custom_value_left = value.customValue;
                    break;
                case 1:
                    master_horse_training_lane_type_mid = value.laneType;
                    custom_value_mid = value.customValue;
                    break;
                case 2:
                    master_horse_training_lane_type_right = value.laneType;
                    custom_value_right = value.customValue;
                    break;
                default:
                    break;
            }
        }
    }
}

#endif