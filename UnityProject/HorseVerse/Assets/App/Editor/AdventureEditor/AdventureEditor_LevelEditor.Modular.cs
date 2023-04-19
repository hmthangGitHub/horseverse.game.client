using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public partial class AdventureEditor_LevelEditor
{
    bool isShowBlockContainerList = false;
    private AdventureEditor_MasterHorseTrainingBlock[] adventureBlockContainers;

    void GUI_ListModular()
    {
        isShowBlockContainerList = EditorGUILayout.BeginFoldoutHeaderGroup(isShowBlockContainerList, "List Block Container (ID  --- Name -- Left -- Mid -- Right)");
        if (isShowBlockContainerList)
        {
            if (blockContainers != default)
            {
                for (int i = 0; i < blockContainers.Length; i++)
                {
                    var item = adventureBlockContainers[i];
                    GUILayout.BeginHorizontal(GUILayout.Width(300));
                    EditorGUILayout.LabelField(item.MasterHorseTrainingBlockId.ToString());
                    item.Name = EditorGUILayout.TextField(item.Name);
                    item.MasterHorseTrainingLaneTypeLeft = (MasterHorseTrainingLaneType)EditorGUILayout.EnumPopup(item.MasterHorseTrainingLaneTypeLeft);
                    item.CustomValueLeft = EditorGUILayout.IntField(item.CustomValueLeft);
                    item.MasterHorseTrainingLaneTypeMid = (MasterHorseTrainingLaneType)EditorGUILayout.EnumPopup(item.MasterHorseTrainingLaneTypeMid);
                    item.CustomValueMid = EditorGUILayout.IntField(item.CustomValueMid);
                    item.MasterHorseTrainingLaneTypeRight = (MasterHorseTrainingLaneType)EditorGUILayout.EnumPopup(item.MasterHorseTrainingLaneTypeRight);
                    item.CustomValueRight = EditorGUILayout.IntField(item.CustomValueRight);

                    if (GUILayout.Button("SaveEdit"))
                    {
                        OnEditBlockContainer(i, item);
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void UpdateListBlockContainer(int len, MasterHorseTrainingBlock[] bl, MasterHorseTrainingBlockCombo[] c)
    {
        blockCombosInContainer = new bool[len];
        
        for (int i = 0; i < len; i++)
        {
            blockCombosInContainer[i] = false;
            var id = c[i].MasterHorseTrainingBlockId;
            if (bl.Any(x => x.MasterHorseTrainingBlockId == id))
            {
                blockCombosInContainer[i] = true;
            }
        }

        var ll = new List<AdventureEditor_MasterHorseTrainingBlock>();
        for(int i = 0; i < bl.Length; i++)
        {
            ll.Add(new AdventureEditor_MasterHorseTrainingBlock(bl[i]));
        }
        adventureBlockContainers = ll.ToArray();
    }

    void OnAddBlockContainer(int index)
    {
#if ENABLE_DEBUG_MODULE
        var ss = blockCombos[index];
        var id = ss.MasterHorseTrainingBlockId;
        if (blockContainers.Any(x => x.MasterHorseTrainingBlockId == id)) return;
        var xs = new MasterHorseTrainingBlock(id, ss.Name,
            MasterHorseTrainingLaneType.Empty, 0,
            MasterHorseTrainingLaneType.Empty, 0,
            MasterHorseTrainingLaneType.Empty, 0);

        masterHorseTrainingBlockContainer.Add(xs);
        blockContainers = masterHorseTrainingBlockContainer.DataList;
        UpdateListBlockContainer(blockCombos.Length, blockContainers, blockCombos);
#endif
    }

    void OnRemoveBlockContainer(int index)
    {
#if ENABLE_DEBUG_MODULE
        var ss = blockCombos[index];
        var id = ss.MasterHorseTrainingBlockId;
        if (!blockContainers.Any(x => x.MasterHorseTrainingBlockId == id)) return;
        masterHorseTrainingBlockContainer.Remove(id);
        blockContainers = masterHorseTrainingBlockContainer.DataList;
        UpdateListBlockContainer(blockCombos.Length, blockContainers, blockCombos);
#endif
    }

    void OnEditBlockContainer(int index, AdventureEditor_MasterHorseTrainingBlock item)
    {
#if ENABLE_DEBUG_MODULE
        item.CopyTo(ref blockContainers[index]);
#endif
    }
}
