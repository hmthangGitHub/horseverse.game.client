using System.Linq;

public partial class MasterTrainingBlockDistributeContainer
{
    private ILookup<long, MasterTrainingBlockDistribute> masterTrainingBlockDistributeIdIndexer;
    
    public ILookup<long, MasterTrainingBlockDistribute> MasterTrainingBlockDistributeIdIndexer => masterTrainingBlockDistributeIdIndexer ??= 
        DataList.ToLookup(x => x.MasterTrainingBlockDistributeId);
}