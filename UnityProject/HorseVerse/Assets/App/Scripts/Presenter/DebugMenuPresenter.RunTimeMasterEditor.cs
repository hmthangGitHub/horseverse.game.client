using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Reflection;
using UnityEngine;

public partial class UIDebugMenuPresenter
{
    private const string MasterEditorMenu = "Master Editor";

    private async UniTask<IEnumerable<(string debugMenu, Action)>> CreateMasterEditionDebugMenu()
    {
        var assembly = typeof(IMasterContainer).Assembly;
        return assembly.GetTypes()
            .Where(x => typeof(IMasterContainer).IsAssignableFrom(x) && !x.IsAbstract)
            .Select(CreateDebugMenu)
            .Append(CreateResetAllLocalMaster());
        await UniTask.CompletedTask;
    }

    private (string debugMenu, Action) CreateResetAllLocalMaster()
    {
        return ($"{MasterEditorMenu}/Reset All Master", PlayerPrefs.DeleteAll);
    }

    private (string debugMenu, Action) CreateDebugMenu(Type type)
    {
        return ($"{MasterEditorMenu}/{type.Name.Replace("Container", "")}", UniTask.Action(async () =>
        {
            await uiDebugMenu.Out();
            using var presenter = new MasterRuntimeEditorPresenter();
            await presenter.PerformMasterEditAsync(type);
            await uiDebugMenu.In();
        }));
    }
}