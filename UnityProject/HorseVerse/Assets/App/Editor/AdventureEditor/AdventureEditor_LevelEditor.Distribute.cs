using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AdventureEditor_LevelEditor
{
    bool isShowBlockDistributeList = false;
    private AdventureEditor_MasterTrainingBlockDistribute[] adventureBlockDistributes;
    private MasterTrainingBlockDistribute[] distributes;
    private long DistributeID = 0;

    void GUI_ListDistribute()
    {
        GUILayout.Space(20);
        GUILayout.Label("DISTRIBUTE");

        GUILayout.BeginHorizontal(GUILayout.Width(300));
        DistributeID = EditorGUILayout.LongField(DistributeID);
        if (GUILayout.Button("Create New Distribute"))
        {
            OnCreateNewDistribute(DistributeID);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        isShowBlockDistributeList = EditorGUILayout.BeginFoldoutHeaderGroup(isShowBlockDistributeList, "List Block Distribute (ID  --- Difficult -- Group ID -- Weight)");
        if (isShowBlockDistributeList)
        {
            if (adventureBlockDistributes != default)
            {
                for (int i = 0; i < adventureBlockDistributes.Length; i++)
                {
                    var item = adventureBlockDistributes[i];
                    GUILayout.BeginHorizontal(GUILayout.Width(300));
                    EditorGUILayout.LabelField(item.MasterTrainingBlockDistributeId.ToString());
                    item.Difficulty = EditorGUILayout.IntField(item.Difficulty);
                    item.MasterHorseTrainingBlockGroupId = EditorGUILayout.IntField(item.MasterHorseTrainingBlockGroupId);
                    item.Weight = EditorGUILayout.IntField(item.Weight);

                    if (GUILayout.Button("SaveEdit"))
                    {
                        OnEditDistribute(i, item);
                    }
                    if (GUILayout.Button("Remove"))
                    {
                        OnRemoveDistribute(i);
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void UpdateListBlockDistribute(MasterTrainingBlockDistribute[] c)
    {
        var ll = new List<AdventureEditor_MasterTrainingBlockDistribute>();
        for (int i = 0; i < c.Length; i++)
        {
            ll.Add(new AdventureEditor_MasterTrainingBlockDistribute(c[i]));
        }
        adventureBlockDistributes = ll.ToArray();
    }

    void OnCreateNewDistribute(long id)
    {
        MasterTrainingBlockDistribute d = new MasterTrainingBlockDistribute(id);
        masterTrainingBlockDistributeContainer.Add(d);
        distributes = masterTrainingBlockDistributeContainer.DataList;
        UpdateListBlockDistribute(distributes);
        id = 0;
    }

    void OnRemoveDistribute(int index)
    {
        var id = distributes[index].MasterTrainingBlockDistributeId;
        masterTrainingBlockDistributeContainer.Remove(id);
        distributes = masterTrainingBlockDistributeContainer.DataList;
        UpdateListBlockDistribute(distributes);
    }

    void OnEditDistribute(int index, AdventureEditor_MasterTrainingBlockDistribute item)
    {
        item.CopyTo(ref distributes[index]);
    }
}
