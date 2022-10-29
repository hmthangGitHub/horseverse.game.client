#if ENABLE_MASTER_RUN_TIME_EDIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;

public class MasterRuntimeEditorPresenter
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);
#endif
    public async UniTask PerformMasterEditAsync<T, TMaster>() where T : IMasterContainer, IMasterContainer<TMaster>, new()
    {
        var uiDebug = await UILoader.Instantiate<UIDebugMasterEditor>();
        var type = typeof(TMaster);
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        uiDebug.SetEntity(new UIDebugMasterEditor.Entity()
        {
            masterColumnList = await CreateMasterColumnEntitiesAsync<T, TMaster>(),
            saveBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                await SaveToLocalAsync<T, TMaster>(uiDebug, fields);
            })),
            resetBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                MasterLoader.Unload<T>();
                PlayerPrefs.DeleteKey(MasterLoader.GetMasterPath<T>());
                uiDebug.masterColumnList.SetEntity(await CreateMasterColumnEntitiesAsync<T, TMaster>());
            })),
            exportBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                var csvDataAsLine = await SaveToLocalAsync<T, TMaster>(uiDebug, fields);
                SaveFile<T, TMaster>(type, csvDataAsLine);
            }))
        });
        await uiDebug.In();
        
    }

    private static void SaveFile<T, TMaster>(Type type, string[] csvDataAsLine)
        where T : IMasterContainer, IMasterContainer<TMaster>, new()
    {
        var fileName = Regex.Replace(type.ToString(), "([A-Z])", "_$1").ToLower().Remove(0, 1);
        var csvDatas = string.Join("\n", csvDataAsLine);
#if UNITY_WEBGL && !UNITY_EDITOR
    var bytes = Encoding.UTF8.GetBytes(csvDatas);
    DownloadFile(string.Empty, string.Empty, "sample.txt", bytes, bytes.Length);
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_MAC || UNITY_STANDALONE_OSX || UNITY_EDITOR
        var path = StandaloneFileBrowser.SaveFilePanel($"Save master", string.Empty, fileName, new[]
        {
            new ExtensionFilter()
            {
                Extensions = new[] { "csv" }
            }
        });
        File.WriteAllText(path, csvDatas);
#endif
    }

    private static async UniTask<string[]> SaveToLocalAsync<T, TMaster>(UIDebugMasterEditor uiDebug, FieldInfo[] fields)
        where T : IMasterContainer, IMasterContainer<TMaster>, new()
    {
        var csvDataAsLine = uiDebug.masterColumnList.entity.entities.Where(x => !x.isHeader)
            .Select((x, i) => (x.value, i))
            .GroupBy(x => x.i / fields.Length)
            .Select(x => string.Join(",", x.Select(x => x.value)))
            .ToArray();
        var headerLine = string.Join(",", fields.Select(x => x.Name));
        var jsonData = CSVFileToJson.ConvertCsvFileToJsonObject(Array.Empty<string>()
            .Append(headerLine)
            .Concat(csvDataAsLine)
            .ToArray());
        PlayerPrefs.SetString(MasterLoader.GetMasterPath<T>(), jsonData);
        MasterLoader.Unload<T>();
        uiDebug.masterColumnList.SetEntity(await CreateMasterColumnEntitiesAsync<T, TMaster>());
        return csvDataAsLine;
    }

    private static async UniTask<UIDebugMasterColumnList.Entity> CreateMasterColumnEntitiesAsync<T, TMaster>() where T : IMasterContainer, IMasterContainer<TMaster>, new()
    {
        var type = typeof(TMaster);
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var masterContainer = await MasterLoader.LoadMasterAsync<T>();
        var headerList = fields.Select(x => new UIDebugMasterColumn.Entity()
        {
            value = x.Name,
            isHeader = true
        });

        var valueList = masterContainer.DataList.SelectMany(master =>
        {
            return fields.Select(x => new UIDebugMasterColumn.Entity()
            {
                value = master.GetPropertyValue(x.Name).ToString(),
                isHeader = false
            });
        });
        return new UIDebugMasterColumnList.Entity()
        {
            entities = headerList.Concat(valueList).ToArray()
        };
    }
}
#endif
