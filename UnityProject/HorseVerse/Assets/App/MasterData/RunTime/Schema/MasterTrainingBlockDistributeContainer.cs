using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterTrainingBlockDistributeContainer : MasterContainer<long, MasterTrainingBlockDistribute>
{
    public MasterTrainingBlockDistributeContainer() : base(x => x.MasterTrainingBlockDistributeId) { }
    public IReadOnlyDictionary<long, MasterTrainingBlockDistribute> MasterTrainingBlockDistributeIndexer => Indexer;
}
