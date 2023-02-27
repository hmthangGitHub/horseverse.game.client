using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterLocalizeContainer : MasterContainer<string, MasterLocalize>
{
    public MasterLocalizeContainer() : base(x => x.MasterLocalizationId) { }
    public IReadOnlyDictionary<string, MasterLocalize> MasterLocalizeIndexer => Indexer;
}
