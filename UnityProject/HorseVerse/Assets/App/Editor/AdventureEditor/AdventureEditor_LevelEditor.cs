using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;
using System.Threading;

public partial class AdventureEditor_LevelEditor : EditorWindow
{
    public TrainingBlockSettings collection;
    public string master_id = "10001003";

    private CancellationTokenSource cts;

    private MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer;
    private MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer;
    private MasterHorseTrainingPropertyContainer masterHorseTrainingPropertyContainer;
    private MasterTrainingModularBlockContainer masterTrainingModularBlockContainer;

    private Vector2 scrollPosition;
    private bool isShowBlockComboList = true;

    [MenuItem("Tools/AdventureEditor/Level Editor")]
    public static void ShowWindows()
    {
        EditorWindow.GetWindow(typeof(AdventureEditor_LevelEditor));
    }

    private void OnDestroy()
    {
        Dispose();
    }

    void OnGUI()
    {
        collection = EditorGUILayout.ObjectField("Training Block Settings", collection, typeof(TrainingBlockSettings), allowSceneObjects: true) as TrainingBlockSettings;
        master_id = EditorGUILayout.TextField("Map ID", master_id);

        if (GUILayout.Button("Refresh"))
        {
            Refresh();
        }

        if (GUILayout.Button("Save Current Data To Map"))
        {
            SaveBlocks();
        }

        GUILayout.Space(20);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.BeginVertical();
        isShowBlockComboList = EditorGUILayout.BeginFoldoutHeaderGroup(isShowBlockComboList, "Block Combo");
        if(isShowBlockComboList)
            GUI_ListBlockCombo();
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void Refresh()
    {
        if (collection == default) return;
        ShowMenuAsync().Forget();
    }

    void SaveBlocks()
    {
        masterHorseTrainingBlockContainer.SaveToLocal();
        masterHorseTrainingBlockComboContainer.SaveToLocal();
    }

    public async UniTask ShowMenuAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        await LoadAssetAsync();
        //await SetEntityAsync();

        InitLevelEditor_BlockCombo();
    }

    private async UniTask LoadAssetAsync()
    {
        //await LoadUIAssets();
        await LoadMasterAsync(master_id);
        await LoadInGameAssetAsync();
    }

    private async UniTask LoadMasterAsync(string id)
    {
        (masterHorseTrainingBlockContainer,
        masterHorseTrainingBlockComboContainer,
        masterHorseTrainingPropertyContainer,
        masterTrainingModularBlockContainer) = await (MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockContainer>(id, cts.Token),
                                                        MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockComboContainer>(id, cts.Token),
                                                        MasterLoader.LoadMasterAsync<MasterHorseTrainingPropertyContainer>(id, cts.Token),
                                                        MasterLoader.LoadMasterAsync<MasterTrainingModularBlockContainer>(id, cts.Token));
    }

    private async UniTask LoadInGameAssetAsync()
    {

    }


    public void Dispose()
    {
        try
        {

        }
        finally
        {
            cts.SafeCancelAndDispose();
            cts = default;

            MasterLoader.SafeRelease(master_id, ref masterHorseTrainingBlockContainer);
            MasterLoader.SafeRelease(master_id, ref masterHorseTrainingBlockComboContainer);
            MasterLoader.SafeRelease(master_id, ref masterHorseTrainingPropertyContainer);

        }
    }

}
