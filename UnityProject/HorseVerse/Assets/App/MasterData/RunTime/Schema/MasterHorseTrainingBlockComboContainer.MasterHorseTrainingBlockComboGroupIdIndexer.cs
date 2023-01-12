using System.Linq;

public partial class MasterHorseTrainingBlockComboContainer
{
    private ILookup<int, MasterHorseTrainingBlockCombo> masterHorseTrainingBlockComboGroupIdIndexer;
    
    public ILookup<int, MasterHorseTrainingBlockCombo> MasterHorseTrainingBlockComboGroupIdIndexer => masterHorseTrainingBlockComboGroupIdIndexer ??= 
        DataList.ToLookup(x => x.MasterHorseTrainingBlockComboGroupId);
}