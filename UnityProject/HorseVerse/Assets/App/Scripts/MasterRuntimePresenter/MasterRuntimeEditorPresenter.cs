#if ENABLE_MASTER_RUN_TIME_EDIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MasterRuntimeEditorPresenter
{
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
                var csvDataAsLine = uiDebug.masterColumnList.entity.entities.Where(x => !x.isHeader)
                    .Select((x, i) => (x.value, i))
                    .GroupBy(x => x.i / fields.Length)
                    .Select(x => string.Join(",", x.Select(x => x.value)))
                    // .Aggregate("", (total, next) => total + "\n" + next)
                    .ToArray();
                var headerLine = string.Join(",", fields.Select(x => x.Name));
                var jsonData = CSVFileToJson.ConvertCsvFileToJsonObject( Array.Empty<string>()
                    .Append(headerLine)
                    .Concat(csvDataAsLine)
                    .ToArray());
                PlayerPrefs.SetString(MasterLoader.GetMasterPath<T>(), jsonData);
                MasterLoader.Unload<T>();
                uiDebug.masterColumnList.SetEntity(await CreateMasterColumnEntitiesAsync<T, TMaster>());
            })),
            resetBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                MasterLoader.Unload<T>();
                PlayerPrefs.DeleteKey(MasterLoader.GetMasterPath<T>());
                uiDebug.masterColumnList.SetEntity(await CreateMasterColumnEntitiesAsync<T, TMaster>());
            }))
        });
        await uiDebug.In();
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
