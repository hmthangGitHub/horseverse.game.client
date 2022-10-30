using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterHorseTrainingPropertyContainer : MasterContainer<long, MasterHorseTrainingProperty>
{
    public MasterHorseTrainingPropertyContainer() : base(x => x.MasterHorseTrainingPropertyId) { }
    public IReadOnlyDictionary<long, MasterHorseTrainingProperty> MasterHorseTrainingPropertyIndexer => Indexer;
}
