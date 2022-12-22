#if ENABLE_MASTER_RUN_TIME_EDIT
using System;
using System.Linq;

public partial class MasterHorseTrainingBlockCombo
{
    public MasterHorseTrainingBlockCombo(long masterHorseTrainingBlockId, string name)
    {
        this.name = name;
        this.master_horse_training_block_id = masterHorseTrainingBlockId;
        master_horse_training_block_ids = string.Empty;
    }
    
    public string[] MasterHorseTrainingBlockIdList
    {
        get => CreateMasterHorseTrainingBlockIdList();
        set => master_horse_training_block_ids = string.Join(".", value);
    }

    private string[] CreateMasterHorseTrainingBlockIdList()
    {
        return !string.IsNullOrEmpty(MasterHorseTrainingBlockIds)
            ? MasterHorseTrainingBlockIds.Split('.')
                .ToArray() 
            : Array.Empty<string>();
    }
}
#endif