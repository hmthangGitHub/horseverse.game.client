using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlatformModular : PlatformBase
{
    [SerializeField]
    private CoinEditor coinPrefab;
    [SerializeField]
    private BoxCollider[] boxColliders;
    [SerializeField]
    private BoxCollider paddingHeadCollider;
    [SerializeField]
    private BoxCollider paddingTailCollider;

    public BoxCollider[] BoxColliders => boxColliders;
    public BoxCollider PaddingHeadCollider => paddingHeadCollider;
    public BoxCollider PaddingTailCollider => paddingTailCollider;
    

    [ContextMenu("Tiling")]
    private void Tiling()
    {
        var sizeZ = BoxColliders.FirstOrDefault()?.bounds.size.z ?? default;
        for (var i = 0; i < BoxColliders.Length; i++)
        {
            ChangePositionOfParentToMatchChildPosition(BoxColliders[i].transform.parent,
                BoxColliders[i].transform,
                new Vector3(0, 0, i * sizeZ));
        }
    }

    [ContextMenu("TilingPaddingBlocks")]
    private void TilingPaddingBlocks()
    {
        var firstBoxCollider = BoxColliders.FirstOrDefault();
        var extendZ = firstBoxCollider?.bounds.extents.z ?? 0;
        
        ChangePositionOfParentToMatchChildPosition(PaddingHeadCollider.transform.parent,
            PaddingHeadCollider.transform,
            new Vector3(0, 0, -(extendZ + PaddingHeadCollider.bounds.extents.z)));
        
        ChangePositionOfParentToMatchChildPosition(PaddingTailCollider.transform.parent,
            PaddingTailCollider.transform,
            new Vector3(0, 0, (BoxColliders.Length - 1) * extendZ * 2 + (extendZ + PaddingHeadCollider.bounds.extents.z)));
    }

    private void PlaceStartObjectAtOffsetToFirstBlock(float offset)
    {
        var boundsExtents = paddingHeadCollider.bounds.extents;
        var localPosition = new Vector3(0, boundsExtents.y, -boundsExtents.z + offset);
        start.transform.position = localPosition + paddingHeadCollider.transform.position;
    }
    
    private void PlaceEndObjectAtOffsetToLastBlock(float offset)
    {
        var boundsExtents = paddingTailCollider.bounds.extents;
        var localPosition = new Vector3(0, boundsExtents.y, boundsExtents.z - offset);
        end.transform.position = localPosition + paddingTailCollider.transform.position;
    }

    private void AlignToStartPosition(Vector3 position)
    {
        ChangePositionOfParentToMatchChildPosition(this.transform, start.transform, position);
    }

    public static void ChangePositionOfParentToMatchChildPosition(Transform parent,
                                                            Transform child,
                                                            Vector3 childWorldDestination)
    {
        parent.position += childWorldDestination - child.position;
    }

    public void GenerateBlock(Vector3 startPosition,
                              GameObject[] blockPrefabs,
                              GameObject paddingHeadPrefab,
                              GameObject paddingTailPrefab,
                              float jumpingPoint,
                              float landingPoint)
    {
        InstantiateBlocks(blockPrefabs, paddingHeadPrefab, paddingTailPrefab);
        Tiling();
        TilingPaddingBlocks();
        PlaceStartObjectAtOffsetToFirstBlock(jumpingPoint);
        PlaceEndObjectAtOffsetToLastBlock(landingPoint);
        AlignToStartPosition(startPosition);
    }
    
    public void GenerateBlock(Vector3 startPosition,
                              GameObject[] blockPrefabs,
                              GameObject paddingHeadPrefab,
                              GameObject paddingTailPrefab,
                              float jumpingPoint,
                              float landingPoint,
                              MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, 
                              GameObject[] obstaclesPrefab)
    {
        GenerateBlock(startPosition, blockPrefabs, paddingHeadPrefab, paddingTailPrefab, jumpingPoint, landingPoint);
        GenerateObstacle(masterHorseTrainingBlockCombo.ObstacleList, obstaclesPrefab);
        GenerateCoins(masterHorseTrainingBlockCombo.CoinList);
    }

    private void GenerateCoins(Coin[] coinsList)
    {
        coinsList.ForEach(x =>
        {
            var coin = Instantiate(coinPrefab, this.transform);
            coin.transform.localPosition = x.localPosition.ToVector3();
            coin.Init(x.numberOfCoin, x.benzierPointPositions.Select(x => x.ToVector3()).ToArray());
        });
     
    }

    private void GenerateObstacle(Obstacle[] obstacleList,
                                  GameObject[] obstaclesPrefab)
    {
        obstacleList.ForEach(x =>
        {
            var obstaclesPrefabParent = obstaclesPrefab.FirstOrDefault(saveObstacles => saveObstacles.name == x.type);
            CreatObstacle(obstaclesPrefabParent, x.localPosition);
        });
    }
    
    private void CreatObstacle(GameObject obstaclesPrefabParent,
                                     Position localPosition)
    {
        var prefab = obstaclesPrefabParent
                     .transform.Cast<Transform>()
                     .Where(x => !x.gameObject.name.Contains("dummy"))
                     .RandomElement();
        var obstacle = UnityEngine.Object.Instantiate(prefab, transform).gameObject;
        obstacle.name = prefab.name;
        obstacle.transform.localPosition = localPosition.ToVector3();
    }

    private void InstantiateBlocks(GameObject[] gameObjects,
                                   GameObject paddingHead,
                                   GameObject paddingTail)
    {
        boxColliders = gameObjects.Select(x => Instantiate(x, this.blockContainer).GetComponentInChildren<BoxCollider>())
                                  .ToArray();
        paddingHeadCollider = Instantiate(paddingHead, this.blockContainer).GetComponentInChildren<BoxCollider>();
        paddingTailCollider = Instantiate(paddingTail, this.blockContainer).GetComponentInChildren<BoxCollider>();
    }
}
