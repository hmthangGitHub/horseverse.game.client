using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterHorseTrainingPropertiesContainer : MasterContainer<long, MasterHorseTrainingProperties>
{
    public MasterHorseTrainingPropertiesContainer() : base(x => x.MasterHorseTrainingProperyId) { }
    public IReadOnlyDictionary<long, MasterHorseTrainingProperties> MasterHorseTrainingPropertiesIndexer => Indexer;
}
