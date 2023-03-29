using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class AdventureEditor_LevelEditor
{
    private readonly List<GameObject> blockComboPinList = new List<GameObject>();
    private MasterHorseTrainingProperty masterHorseTrainingProperty;

    private MasterHorseTrainingBlockCombo[] blockCombos;
    private int CurrentSelectetBlockCombo = -1;
    void InitLevelEditor_BlockCombo() {
        blockCombos = GetCurrentBlockCombo(MasterTrainingBlockComboType.Modular);
    }


    void GUI_ListBlockCombo()
    {
        if (blockCombos == default) return;

        for (int i = 0; i < blockCombos.Length; i++)
        {
            var combo = blockCombos[i];
            GUILayout.BeginHorizontal(GUILayout.Width(300));

            EditorGUILayout.LabelField(combo.Name);
            EditorGUILayout.Toggle(CurrentSelectetBlockCombo == i);
            if (GUILayout.Button("Edit"))
            {
                CurrentSelectetBlockCombo = i;
            }

            GUILayout.EndHorizontal();
        }
    }

    MasterHorseTrainingBlockCombo[] GetCurrentBlockCombo(MasterTrainingBlockComboType CurrentBlockComboType)
    {
        return masterHorseTrainingBlockComboContainer.DataList.Where(x => x.MasterTrainingBlockComboType == CurrentBlockComboType).ToArray();
    }


}
