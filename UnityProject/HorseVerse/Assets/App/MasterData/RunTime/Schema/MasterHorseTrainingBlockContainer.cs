using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterHorseTrainingBlockContainer : MasterContainer<long, MasterHorseTrainingBlock>
{
    public MasterHorseTrainingBlockContainer() : base(x => x.MasterHorseTrainingBlockId) { }
    public IReadOnlyDictionary<long, MasterHorseTrainingBlock> MasterHorseTrainingBlockIndexer => Indexer;
}
