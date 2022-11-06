using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if ENABLE_MASTER_RUN_TIME_EDIT

public partial interface IMasterContainer
{
    void SaveToLocal();
}

public partial class MasterContainer<TKey, TMaster>
{
    public void SaveToLocal()
    {
        var type = MasterType;
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var headerLine = string.Join(",", fields.Select(x => x.Name));
        var csvDataAsLine = DataList.Select(master => 
                fields.Select(x => master.GetPropertyValue(x.Name).ToString()))
            .Select(dataInRow => string.Join(",", dataInRow)).ToList();
        var jsonData = CSVFileToJson.ConvertCsvFileToJsonObject(Array.Empty<string>()
            .Append(headerLine)
            .Concat(csvDataAsLine)
            .ToArray());
        PlayerPrefs.SetString(MasterLoader.GetMasterPath(this.GetType()), jsonData);
    }
}
#endif
