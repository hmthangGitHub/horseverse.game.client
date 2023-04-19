using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class AdventureEditor_LevelEditor
{
    private async UniTask GenerateObstacle(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, Transform parent, AdventureEditor_BlockComboData data)
    {
        await UniTask.Yield();
        var obstacles = collection.obstacles.ToList();
        masterHorseTrainingBlockCombo.ObstacleList
                                  .ForEach(x =>
                                  {
                                      var b = CreateObstacleAtPosition(
                                          obstacles.FindIndex(saveObstacles => saveObstacles.name == x.type),
                                          x.localPosition.ToVector3(), 
                                          parent);
                                      data.obstabcles.Add(b);
                                  });
    }

    private GameObject CreateObstacleAtPosition(int index, Vector3 localPosition, Transform parent)
    {
        var obstacle = CreatObstacle(index, parent);
        obstacle.transform.localPosition = new Vector3(localPosition.x, obstacle.transform.localPosition.y, localPosition.z);
        return obstacle;
    }

    private GameObject CreatObstacle(int index, Transform parent)
    {
        var prefab = collection.obstacles[index]
                             .transform.Cast<Transform>()
                             .First(x => x.gameObject.name.Contains("dummy"));
        var gameOb = new GameObject();
        gameOb.transform.parent = parent;
        gameOb.name = collection.obstacles[index].name;
        var obstacleDummy = UnityEngine.Object.Instantiate(prefab, gameOb.transform).gameObject;
        obstacleDummy.name = prefab.name;
        return gameOb;
    }

    private void SaveObstacleToBlock(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, List<GameObject> obstacles)
    {
#if ENABLE_DEBUG_MODULE
        masterHorseTrainingBlockCombo.ObstacleList = obstacles.Select(x =>
                new Obstacle()
                {
                    type = x.name,
                    localPosition = Position.FromVector3(x.transform.localPosition)
                })
            .ToArray();
#endif
    }

}
