using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterErrorCodeContainer : MasterContainer<int, MasterErrorCode>
{
    public MasterErrorCodeContainer() : base(x => x.MasterErrorCodeId) { }
    public IReadOnlyDictionary<int, MasterErrorCode> MasterErrorCodeIndexer => Indexer;
}
