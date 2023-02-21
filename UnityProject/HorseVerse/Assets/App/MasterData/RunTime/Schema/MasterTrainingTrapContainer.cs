using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterTrainingTrapContainer : MasterContainer<string, MasterTrainingTrap>
{
    public MasterTrainingTrapContainer() : base(x => x.MasterTrainingTrapId) { }
    public IReadOnlyDictionary<string, MasterTrainingTrap> MasterTrainingTrapIndexer => Indexer;
}
