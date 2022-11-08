using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterHorseTrainingBlockComboContainer : MasterContainer<long, MasterHorseTrainingBlockCombo>
{
    public MasterHorseTrainingBlockComboContainer() : base(x => x.MasterHorseTrainingBlockId) { }
    public IReadOnlyDictionary<long, MasterHorseTrainingBlockCombo> MasterHorseTrainingBlockComboIndexer => Indexer;
}
