using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterHorseContainer : MasterContainer<long, MasterHorse>
{
    public MasterHorseContainer() : base(x => x.MasterHorseId) { }
    public IReadOnlyDictionary<long, MasterHorse> MasterHorseIndexer => Indexer;
}
