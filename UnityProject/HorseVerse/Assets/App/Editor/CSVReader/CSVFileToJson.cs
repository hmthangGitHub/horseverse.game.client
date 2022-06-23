using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVFileToJson
{
    public static string ConvertCsvFileToJsonObject(string[] csvDataAsLines)
    {
        var csv = new List<string[]>();

        foreach (string line in csvDataAsLines)
            csv.Add(line.Split(','));

        var properties = csvDataAsLines[0].Split(',');

        var listObjResult = new List<Dictionary<string, object>>();

        for (int i = 1; i < csvDataAsLines.Length; i++)
        {
            var objResult = new Dictionary<string, object>();
            for (int j = 0; j < properties.Length; j++)
                objResult.Add(properties[j], csv[i][j]);

            listObjResult.Add(objResult);
        }

        var json = JsonConvert.SerializeObject(listObjResult, Formatting.Indented);
        return json;
    }

}
