using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GoogleSheetsToUnity;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.Windows;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using File = System.IO.File;

public partial class MasterDataGenerator
{
    private static async UniTask<BatchRawData> BatchReadRawAsync(string sheetId,
                                                                 (string name, string path)[] sheetRanges,
                                                                 bool valueAsFormatted,
                                                                 CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(SpreadsheetManager.Config.gdr?.refresh_token))
        {
            GoogleAuthrisationHelper.BuildHttpListener();
        }
        await SpreadsheetManager.CheckForRefreshToken();

        var valueRenderOption = valueAsFormatted ? "FORMATTED_VALUE" : "UNFORMATTED_VALUE";
        var sheetRangeTexts = string.Join("&", sheetRanges.Select(x => $"ranges='{x.name}'"));
        var url = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values:batchGet?{sheetRangeTexts}&valueRenderOption={valueRenderOption}&access_token={SpreadsheetManager.Config.gdr.access_token}";

        var batchRawData = await GetAsync<BatchRawData>(url, default);
        sheetRanges.ToList()
                   .ForEach(x => WriteCsvFile(batchRawData.valueRanges.First(rawData => rawData.range.Contains(x.name)), 
                       x.path));
        return batchRawData;
    }

    private static void WriteCsvFile(RawData rawData, string path)
    {
        var columnLength = rawData.values.First()
                                  .Count;
        var masterData = string.Join("\n", rawData.values
                                                  .Select(x => x.Concat(Enumerable.Range(0, columnLength - x.Count)
                                                                                        .Select(dummy => string.Empty))
                                                  )
                                                  .Select(x => string.Join(",", x)));
        File.WriteAllText(path, masterData);
    }

    private static async UniTask<T> GetAsync<T>(string url,
                                                CancellationToken ct)
    {
        using var request = UnityWebRequest.Get(url);
        await request.SendWebRequest()
                     .ToUniTask(cancellationToken: ct);

        if (string.IsNullOrEmpty(request.downloadHandler.text) || request.downloadHandler.text == "{}")
        {
            Debug.LogWarning("Unable to Retrieve data from google sheets");
            return default;
        }

        return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
    }

    [Serializable]
    internal class BatchRawData
    {
        public string spreadsheetId = string.Empty;

        public List<RawData> valueRanges = Enumerable.Empty<RawData>()
                                                     .ToList();
    }

    [Serializable]
    internal class RawData
    {
        public string range = "";
        public string majorDimension = default;
        public IReadOnlyList<IReadOnlyList<string>> values = new List<List<string>>();
    }
}