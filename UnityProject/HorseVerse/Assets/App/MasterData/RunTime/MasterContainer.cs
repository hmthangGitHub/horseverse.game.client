using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IMasterContainer
{
    Type MasterType { get; }
    void SetDataList(string jsonDataList);
}

public interface IMasterContainer<TMaster>
{
    public TMaster[] DataList { get;}
}

public abstract class MasterContainer<TKey, TMaster> : IMasterContainer, IMasterContainer<TMaster>
{
    public TMaster[] DataList { get; private set; }
    private Func<TMaster, TKey> keyPredictor = default;
    private Dictionary<TKey, TMaster> indexer = default;
    protected IReadOnlyDictionary<TKey, TMaster> Indexer => indexer ??= DataList.ToDictionary(keyPredictor);

    public MasterContainer(Func<TMaster, TKey> keyPredictor)
    {
        this.keyPredictor = keyPredictor;
    }

    public void SetDataList(string jsonDataList)
    {
        DataList = JsonConvert.DeserializeObject(jsonDataList, typeof(TMaster[])) as TMaster[];
    }

    public Type MasterType => typeof(TMaster);
}