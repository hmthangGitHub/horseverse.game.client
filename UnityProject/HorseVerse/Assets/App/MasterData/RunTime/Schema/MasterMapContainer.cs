using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterMapContainer : MasterContainer<long, MasterMap>
{
    public MasterMapContainer() : base(x => x.MasterMapId) { }
    public IReadOnlyDictionary<long, MasterMap> MasterMapIndexer => Indexer;
}
