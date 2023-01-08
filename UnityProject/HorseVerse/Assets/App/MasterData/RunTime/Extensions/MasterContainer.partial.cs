#if ENABLE_MASTER_RUN_TIME_EDIT
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public partial class MasterContainer<TKey, TMaster>
{
    public void Remove(TKey key)
    {
        var dataList = DataList.ToList();
        dataList.Remove(Indexer[key]);
        DataList = dataList.ToArray();
        indexer = default;
    }
    
    public void Add(TMaster master)
    {
        var dataList = DataList.ToList();
        dataList.Add(master);
        DataList = dataList.ToArray();
        indexer = default;
    }

    public void AddOrModified(TMaster master)
    {
        var key = keyPredictor(master);
        if (Indexer.TryGetValue(key, out var oldMaster))
        {
            var oldDataList = DataList.ToList();
            var index = oldDataList.FindIndex(x => x.Equals(oldMaster));
            oldDataList[index] = master;
            DataList = oldDataList.ToArray();
            indexer = default;
        }
        else
        {
            Add(master);
        }
        
    }
}
#endif