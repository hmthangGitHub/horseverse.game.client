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
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;

public class MasterRuntimeEditorPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private UIDebugMasterEditor uiDebugMasterEditor;
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);
#endif
    public async UniTask PerformMasterEditAsyncGeneric<T>() where T : IMasterContainer, new()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        var ucs = new UniTaskCompletionSource();
        uiDebugMasterEditor ??= await UILoader.Instantiate<UIDebugMasterEditor>(UICanvas.UICanvasType.Debug, token : cts.Token);
        var type = GetMasterTypeFromMasterContainer<T>();
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        uiDebugMasterEditor.SetEntity(new UIDebugMasterEditor.Entity()
        {
            masterColumnList = await CreateMasterColumnEntitiesAsync<T>(),
            saveBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                await SaveToLocalAsync<T>(uiDebugMasterEditor, fields);
            })),
            resetBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                MasterLoader.Unload<T>();
                PlayerPrefs.DeleteKey(MasterLoader.GetMasterPath<T>());
                uiDebugMasterEditor.masterColumnList.SetEntity(await CreateMasterColumnEntitiesAsync<T>());
            })),
            exportBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                var csvDataAsLine = await SaveToLocalAsync<T>(uiDebugMasterEditor, fields);
                SaveFile<T>(type, csvDataAsLine);
            })),
            title = type.Name,
            closeBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            })
        });
        await uiDebugMasterEditor.In().AttachExternalCancellation(cts.Token);
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public async UniTask PerformMasterEditAsync(Type type)
    {
        var method = typeof(MasterRuntimeEditorPresenter).GetMethod(nameof(PerformMasterEditAsyncGeneric));
        var generic = method.MakeGenericMethod(type);
        await (UniTask)(generic.Invoke(this, null));
    }

    private static Type GetMasterTypeFromMasterContainer<T>() where T : IMasterContainer, new()
    {
        var genericArguments = typeof(T).BaseType.GetGenericArguments();
        var type = genericArguments[1];
        return type;
    }
    

    private static void SaveFile<T>(Type type, string[] csvDataAsLine)
        where T : IMasterContainer, new()
    {
        var fileName = Regex.Replace(type.ToString(), "([A-Z])", "_$1").ToLower().Remove(0, 1);
        var csvDatas = string.Join("\n", csvDataAsLine);
        
#if UNITY_WEBGL && !UNITY_EDITOR
    var bytes = Encoding.UTF8.GetBytes(csvDatas);
    DownloadFile(string.Empty, string.Empty, $"{fileName}.csv", bytes, bytes.Length);
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_MAC || UNITY_STANDALONE_OSX || UNITY_EDITOR
        var path = StandaloneFileBrowser.SaveFilePanel($"Save master", string.Empty, fileName, new[]
        {
            new ExtensionFilter()
            {
                Extensions = new[] { "csv" }
            }
        });
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, csvDatas);
        }
#endif
    }

    private static async UniTask<string[]> SaveToLocalAsync<T>(UIDebugMasterEditor uiDebug, FieldInfo[] fields)
        where T : IMasterContainer, new()
    {
        var csvDataAsLine = uiDebug.masterColumnList.entity.entities.Where(x => !x.isHeader)
            .Select((x, i) => (x.value, i))
            .GroupBy(x => x.i / fields.Length)
            .Select(x => string.Join(",", x.Select(value => value.value)))
            .ToArray();
        var headerLine = string.Join(",", fields.Select(x => x.Name));
        var jsonData = CSVFileToJson.ConvertCsvFileToJsonObject(Array.Empty<string>()
            .Append(headerLine)
            .Concat(csvDataAsLine)
            .ToArray());
        PlayerPrefs.SetString(MasterLoader.GetMasterPath<T>(), jsonData);
        MasterLoader.Unload<T>();
        uiDebug.masterColumnList.SetEntity(await CreateMasterColumnEntitiesAsync<T>());
        return csvDataAsLine;
    }

    private static async UniTask<UIDebugMasterColumnList.Entity> CreateMasterColumnEntitiesAsync<T>() where T : IMasterContainer, new()
    {
        var type = GetMasterTypeFromMasterContainer<T>();
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var masterContainer = await MasterLoader.LoadMasterAsync<T>();
        var headerList = fields.Select(x => new UIDebugMasterColumn.Entity()
        {
            value = x.Name,
            isHeader = true
        });

        var dataList = masterContainer.GetPropertyValue("DataList") as IEnumerable;
        var valueList = new List<UIDebugMasterColumn.Entity>();
        foreach (var master in dataList)
        {
            var dataInRow = fields.Select(x => new UIDebugMasterColumn.Entity()
            {
                value = master.GetPropertyValue(x.Name).ToString(),
                isHeader = false
            });
            valueList.AddRange(dataInRow);
        }
        return new UIDebugMasterColumnList.Entity()
        {
            entities = headerList.Concat(valueList).ToArray()
        };
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        UILoader.SafeRelease(ref uiDebugMasterEditor);
    }
}
#endif
