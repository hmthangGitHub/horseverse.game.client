using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterTrainingModularBlockContainer : MasterContainer<string, MasterTrainingModularBlock>
{
    public MasterTrainingModularBlockContainer() : base(x => x.MasterTrainingModularBlockId) { }
    public IReadOnlyDictionary<string, MasterTrainingModularBlock> MasterTrainingModularBlockIndexer => Indexer;
}
