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

    private Vector3[] centers;

    public BoxCollider[] BoxColliders => boxColliders;
    public BoxCollider PaddingHeadCollider => paddingHeadCollider;
    public BoxCollider PaddingTailCollider => paddingTailCollider;
    

    [ContextMenu("Tiling")]
    private void Tiling()
    {
        if (BoxColliders.Length <= 0) return;
        ChangePositionOfParentToMatchChildPosition(BoxColliders[0].transform.parent,
            BoxColliders[0].transform,
            new Vector3(0, 0, 0));

        centers = BoxColliders.Select(x => x.center)
                                  .ToArray();
        for (var i = 1; i < BoxColliders.Length; i++)
        {
            var baseCollider = BoxColliders[i - 1];
            var alignedCollider = BoxColliders[i];
            
            AlignCollider(baseCollider, alignedCollider, 1);
        }
    }

    private static void AlignCollider(BoxCollider baseCollider,
                                      BoxCollider alignedCollider,
                                      int direction)
    {
        var worldPos = baseCollider.transform.position + baseCollider.center +
                       new Vector3(0, baseCollider.bounds.extents.y, direction * baseCollider.bounds.extents.z)
                       - (alignedCollider.center + new Vector3(0, alignedCollider.bounds.extents.y, -direction * alignedCollider.bounds.extents.z));
        ChangePositionOfParentToMatchChildPosition(alignedCollider.transform.parent,
            alignedCollider.transform,
            worldPos);
    }

    [ContextMenu("TilingPaddingBlocks")]
    private void TilingPaddingBlocks()
    {
        if (!BoxColliders.Any())
        {
            ChangePositionOfParentToMatchChildPosition(PaddingHeadCollider.transform.parent,
                PaddingHeadCollider.transform,
                new Vector3(0, 0, -(0 + PaddingHeadCollider.bounds.extents.z)));
        
            ChangePositionOfParentToMatchChildPosition(PaddingTailCollider.transform.parent,
                PaddingTailCollider.transform,
                new Vector3(0, 0, (BoxColliders.Length - 1) * 0 * 2 + (0 + PaddingHeadCollider.bounds.extents.z)));
        }
        else
        {
            AlignCollider(BoxColliders.First(),PaddingHeadCollider, -1);
            AlignCollider(BoxColliders.Last(),PaddingTailCollider, 1);
        }
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
    
    public static void Snap(Collider floor, Collider objetToSnap)
    {
        var bounds = floor.bounds;
        var yHeadOffset = bounds.center.y + bounds.extents.y;
        var obstacleBounds = objetToSnap.bounds;
        var yObstacleOffset = -obstacleBounds.center.y + obstacleBounds.extents.y;
        objetToSnap.transform.position = floor.transform.position
                                         + Vector3.up * (yHeadOffset + yObstacleOffset);
    }
}
