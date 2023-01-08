using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MasterCoinPresetContainer : MasterContainer<string, MasterCoinPreset>
{
    public MasterCoinPresetContainer() : base(x => x.MasterCoinPresetId) { }
    public IReadOnlyDictionary<string, MasterCoinPreset> MasterCoinPresetIndexer => Indexer;
}
