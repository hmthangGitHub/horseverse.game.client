using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;

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
    
    [MenuItem("Tools/AdventureEditor/Level Editor")]
    public static void ShowWindows()
    {
        EditorWindow.GetWindow(typeof(AdventureEditor_LevelEditor));
    }

    private void OnDestroy()
    {
        Dispose();
        Debug.Log("OnDestroy ");
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
        GUI_ListBlockCombo();
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void Refresh()
    {
        if (collection == default) return;

#if UNITY_EDITOR
        string path = EditorUtility.OpenFolderPanel("Master Data Folder", "", "");
        if (path.Length != 0)
        {
            ShowMenuAsync(path).Forget();
        }
#endif

    }

    void SaveBlocks()
    {
#if ENABLE_DEBUG_MODULE
        masterHorseTrainingBlockContainer.SaveToLocal(master_id);
        masterHorseTrainingBlockComboContainer.SaveToLocal(master_id);

#if UNITY_EDITOR
        var dataBlock = masterHorseTrainingBlockContainer.ToJson();
        var dataContainer = masterHorseTrainingBlockComboContainer.ToJson();

        string path = EditorUtility.OpenFolderPanel("Save Data Folder", "", "");
        if (path.Length != 0)
        {
            SaveFile($"{path}/MasterHorseTrainingBlock_{master_id}.json", dataBlock);
            SaveFile($"{path}/MasterHorseTrainingBlockCombo_{master_id}.json", dataContainer);
        }

#endif

#endif
    }

    public async UniTask ShowMenuAsync(string masterFolder)
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        await LoadMasterAsync(masterFolder, master_id);

        InitLevelEditor_BlockCombo();
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

    private async UniTask LoadMasterAsync(string folder, string id)
    {
        string path_1 = $"{folder}/MasterHorseTrainingBlock_{id}.json";
        string path_2 = $"{folder}/MasterHorseTrainingBlockCombo_{id}.json";
        string path_3 = $"{folder}/MasterHorseTrainingProperty_{id}.json";
        string path_4 = $"{folder}/MasterTrainingModularBlock_{id}.json";
        var text_1 = LoadFile(path_1);
        var text_2 = LoadFile(path_2);
        var text_3 = LoadFile(path_3);
        var text_4 = LoadFile(path_4);

        (masterHorseTrainingBlockContainer,
        masterHorseTrainingBlockComboContainer,
        masterHorseTrainingPropertyContainer,
        masterTrainingModularBlockContainer) = (MasterLoader.LoadMasterFromTextAsync<MasterHorseTrainingBlockContainer>(text_1, cts.Token),
                                                        MasterLoader.LoadMasterFromTextAsync<MasterHorseTrainingBlockComboContainer>(text_2, cts.Token),
                                                        MasterLoader.LoadMasterFromTextAsync<MasterHorseTrainingPropertyContainer>(text_3, cts.Token),
                                                        MasterLoader.LoadMasterFromTextAsync<MasterTrainingModularBlockContainer>(text_4, cts.Token));
    }

    public void Dispose()
    {
        try
        {
            UnSelectOldBlockCombo();
        }
        finally
        {
            cts.SafeCancelAndDispose();
            cts = default;

            //MasterLoader.SafeRelease(master_id, ref masterHorseTrainingBlockContainer);
            //MasterLoader.SafeRelease(master_id, ref masterHorseTrainingBlockComboContainer);
            //MasterLoader.SafeRelease(master_id, ref masterHorseTrainingPropertyContainer);

        }
    }

    public static string LoadFile(string path)
    {
#if UNITY_EDITOR
        //SAVE FILE
        string fileContents = File.ReadAllText(path);
        return fileContents;
#else
        return "";
#endif
    }

    public static void SaveFile(string path, string data)
    {
#if UNITY_EDITOR
        //SAVE FILE
        File.WriteAllText(path, data);
#endif
    }


}
