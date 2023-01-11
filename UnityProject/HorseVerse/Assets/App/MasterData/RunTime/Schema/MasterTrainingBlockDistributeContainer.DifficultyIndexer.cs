using System.Linq;

public partial class MasterTrainingBlockDistributeContainer
{
    private ILookup<int, MasterTrainingBlockDistribute> difficultyIndexer;
    
    public ILookup<int, MasterTrainingBlockDistribute> DifficultyIndexer => difficultyIndexer ??= 
        DataList.ToLookup(x => x.Difficulty);
}