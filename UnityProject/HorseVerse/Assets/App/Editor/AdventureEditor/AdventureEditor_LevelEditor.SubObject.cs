using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class AdventureEditor_LevelEditor
{
    private async UniTask GenerateSubObjects(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, Transform parent, AdventureEditor_BlockComboData data)
    {
        await UniTask.Yield();
        if (collection.subObjects == default) return;
        var subObjects = collection.subObjects.ToList();
        masterHorseTrainingBlockCombo.SubObjectList
                                  .ForEach(x =>
                                  {
                                      var b = CreateSubObjectAtPosition(
                                          subObjects.FindIndex(saveObjs => saveObjs.name == x.type),
                                          x.localPosition.ToVector3(),
                                          parent);
                                      data.subObjects.Add(b);
                                  });
    }

    private GameObject CreateSubObjectAtPosition(int index, Vector3 localPosition, Transform parent)
    {
        var obj = CreatSubObject(index, parent);
        obj.transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
        return obj;
    }

    private GameObject CreatSubObject(int index, Transform parent)
    {
        var prefab = collection.subObjects[index];
        var gameOb = UnityEngine.Object.Instantiate(prefab, parent).gameObject;
        gameOb.name = collection.subObjects[index].name;
        return gameOb;
    }

    private void SaveSubObjectsToBlock(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, List<GameObject> subObjects)
    {
        masterHorseTrainingBlockCombo.SubObjectList = subObjects.Select(x =>
                new SubObject()
                {
                    type = x.name,
                    localPosition = Position.FromVector3(x.transform.localPosition)
                })
            .ToArray();
    }
}
