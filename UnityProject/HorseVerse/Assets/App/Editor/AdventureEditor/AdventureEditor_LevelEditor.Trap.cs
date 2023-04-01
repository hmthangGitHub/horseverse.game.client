using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class AdventureEditor_LevelEditor
{
    private async UniTask GenerateTrap(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, Transform parent, AdventureEditor_BlockComboData data)
    {
        await UniTask.Yield();
        var traps = collection.traps.ToList();
        masterHorseTrainingBlockCombo.TrapList
                                  .ForEach(x =>
                                  {
                                      var trap = CreateTrapAtPosition(parent, 
                                          traps.FindIndex(saveTraps => saveTraps.name == $"{x.type}_{x.id}"),
                                          x.localPosition.ToVector3(), x.extraData);
                                      data.traps.Add(trap);
                                  });
    }

    private TrapEditor CreateTrapAtPosition(Transform parent, int index, Vector3 localPosition, string extraData = "")
    {
        var trap = CreatTrap(index, parent);
        trap.transform.localPosition = new Vector3(localPosition.x, trap.transform.localPosition.y, localPosition.z);
        if (!string.IsNullOrEmpty(extraData))
        {
            if (trap.Type == TrapEditor.TYPE.ROLLING_ROCK)
            {
                var comp = trap.gameObject.GetComponent<TrapEditorRollingStone>();
                comp.SetExtraData(extraData);
            }
            else if (trap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
            {
                var comp = trap.gameObject.GetComponent<TrapEditorWoodSpike>();
                comp.SetExtraData(extraData);
            }
        }
        return trap;
    }

    private TrapEditor CreatTrap(int index, Transform parent)
    {
        if (collection.trapEditors.Length == 0)
        {
            Debug.LogError("Null Trap Editor");
            return null;
        }
        var prefab = collection.trapEditors[index].transform;
        var trapDummy = UnityEngine.Object.Instantiate(prefab, parent).gameObject;
        trapDummy.name = prefab.name;
        var trapEditor = trapDummy.GetComponent<TrapEditor>();
        return trapEditor;
    }

    private void SaveTrapToBlock(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, List<TrapEditor> traps)
    {
        masterHorseTrainingBlockCombo.TrapList = traps.Select(x => {
            var trapEdi = x;
            return new Trap()
            {
                type = trapEdi.Type.ToString(),
                eType = (int)trapEdi.Type,
                id = trapEdi.TrapID,
                localPosition = Position.FromVector3(x.transform.localPosition),
                extraData = trapEdi.getExtraData(),
            };
        }).ToArray();
    }
}
