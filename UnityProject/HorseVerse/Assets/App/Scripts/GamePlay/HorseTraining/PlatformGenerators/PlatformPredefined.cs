using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformPredefined : PlatformBase
{
    [SerializeField]
    private TrainingCoin coinPrefab;
    
    public virtual void GenerateBlocks(Vector3 relativePointToPlayer,
                                       Vector3 lastEndPosition,
                                       TrainingBlockPredefine blockComboPrefab,
                                       GameObject[] obstacleVariationContainer,
                                       GameObject[] sceneryPrefab,
                                       MasterHorseTrainingProperty masterHorseTrainingProperty)
    {
        var blockCombo = InstantiateTrainingBlockPredefine(relativePointToPlayer, lastEndPosition, blockComboPrefab, masterHorseTrainingProperty);
        InstantiateObstacles(obstacleVariationContainer, blockCombo);
        InstantiateScenery(sceneryPrefab, blockCombo);
        InstantiateCoins(blockCombo, masterHorseTrainingProperty);
    }

    private void InstantiateCoins(TrainingBlockPredefine trainingBlockPredefine,
                                  MasterHorseTrainingProperty masterHorseTrainingProperty)
    {
        trainingBlockPredefine.coinPositions.ForEach(x =>
        {
            var coin = Instantiate(coinPrefab, x, Quaternion.identity, trainingBlockPredefine.transform);
            coin.transform.localPosition = x + new Vector3(0 , 1, 0);
            coinPrefab.GetComponent<SphereCollider>().radius = masterHorseTrainingProperty.CoinColliderRadius;
        });
    }

    private static void InstantiateScenery(GameObject[] sceneryPrefab,
                                           TrainingBlockPredefine blockCombo)
    {
        Enumerable.Range(0, Random.Range(0, 8))
                  .ForEach(x =>
                  {
                      var p = blockCombo.sceneryContainer.bounds.RandomPointInBounds();
                      while (blockCombo.sceneryConflictRegion.bounds.Contains(p))
                      {
                          p = blockCombo.sceneryContainer.bounds.RandomPointInBounds();
                      }

                      var sceneryObject = Instantiate(sceneryPrefab.RandomElement(), p + blockCombo.transform.position,
                          Quaternion.identity, blockCombo.transform);
                  });
    }

    private static void InstantiateObstacles(GameObject[] obstacleVariationContainer,
                                             TrainingBlockPredefine blockCombo)
    {
        blockCombo.obstacleDummies
                  .ForEach(x =>
                  {
                      var position = x.transform.position;
                      Destroy(x.gameObject);
                      var obstacle = obstacleVariationContainer.FirstOrDefault(obstacle => x.name.Contains(obstacle.name))
                                                               .transform
                                                               .Cast<Transform>()
                                                               .Where(obstacle => !obstacle.name.Contains("dummy"))
                                                               .RandomElement();
                      Instantiate(obstacle, x.transform.parent)
                          .transform.position = position;
                  });
    }

    private TrainingBlockPredefine InstantiateTrainingBlockPredefine(Vector3 relativePointToPlayer,
                                                                     Vector3 lastEndPosition,
                                                                     TrainingBlockPredefine blockComboPrefab,
                                                                     MasterHorseTrainingProperty
                                                                         masterHorseTrainingProperty)
    {
        var blockCombo = Instantiate(blockComboPrefab, this.transform);
        var platform = blockCombo.GetComponentInChildren<BoxCollider>();
        var platformBounds = platform.bounds;
        var startPositionLocal = new Vector3(0, platformBounds.extents.y,
            -platformBounds.extents.z + masterHorseTrainingProperty.LandingPoint);
        var endPositionLocal = new Vector3(0, platformBounds.extents.y, platformBounds.extents.z - masterHorseTrainingProperty.JumpingPoint);
        blockCombo.transform.position = lastEndPosition + relativePointToPlayer - startPositionLocal - platform.transform.localPosition;
        start.transform.position = lastEndPosition + relativePointToPlayer;
        end.transform.position = platform.transform.position + endPositionLocal;
        return blockCombo;
    }
}
