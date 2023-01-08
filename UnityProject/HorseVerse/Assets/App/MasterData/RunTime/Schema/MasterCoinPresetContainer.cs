using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterCoinPresetContainer : MasterContainer<long, MasterCoinPreset>
{
    public MasterCoinPresetContainer() : base(x => x.MasterMapId) { }
    public IReadOnlyDictionary<long, MasterCoinPreset> MasterCoinPresetIndexer => Indexer;
}
