using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVFileToJson
{
    public static string ConvertCsvFileToJsonObject(string path)
    {
        var lines = new string[]
        {
            "master_horse_id,model_path,name",
            "10000001,Horses/Horse_Black,Black",
            "10000002,Horses/Horse_Black_Tobiano_pinto,Black Tobiano",
            "10000003,Horses/Horse_Brown,Brown",
            "10000004,Horses/Horse_Buckskin,Buckskin",
            "10000005,Horses/Horse_GraysRoans,GraysRoans",
            "10000006,Horses/Horse_Palomino,Palomino",
            "10000007,Horses/Horse_palomino_overo_pinto,Overo Pinto",
            "10000008,Horses/Horse_White,White"
        };

        var csv = new List<string[]>();

        foreach (string line in lines)
            csv.Add(line.Split(','));

        var properties = lines[0].Split(',');

        var listObjResult = new List<Dictionary<string, object>>();

        for (int i = 1; i < lines.Length; i++)
        {
            var objResult = new Dictionary<string, object>();
            for (int j = 0; j < properties.Length; j++)
                objResult.Add(properties[j], csv[i][j]);

            listObjResult.Add(objResult);
        }

        var json = JsonConvert.SerializeObject(listObjResult);
        return json;
    }

}
