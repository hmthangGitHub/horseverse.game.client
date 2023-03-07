using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public partial class PlatformModular : PlatformBase
{
#if ENABLE_DEBUG_MODULE
    public TextMeshPro blockName;
    public void SetBlockName(string name)
    {
        blockName.transform.position = FirstCollider.transform.position + Vector3.up * 5.0f;
        blockName.text = name;
        this.gameObject.name = name;
        this.gameObject.SetActive(true);
    }
#endif
    
    [SerializeField]
    private CoinEditor coinPrefab;
    [SerializeField]
    private BoxCollider[] boxColliders;
    [SerializeField]
    private List<BoxCollider> allPlatformColliders = new List<BoxCollider>();
    [SerializeField]
    private BoxCollider paddingHeadCollider;
    [SerializeField]
    private BoxCollider paddingTailCollider;
    [SerializeField]
    public BoxCollider sceneryBoxContainer;
    [SerializeField]
    public BoxCollider sceneryConflictRegion;
    
    private PlatformGeneratorPool pool;

    private Vector3[] centers;

    public BoxCollider[] BoxColliders => boxColliders;
    public BoxCollider PaddingHeadCollider => paddingHeadCollider;
    public BoxCollider PaddingTailCollider => paddingTailCollider;
    public BoxCollider FirstCollider => allPlatformColliders.First();
    public BoxCollider LastCollider => allPlatformColliders.Last();

    private readonly List<IPoolableDisposableObject> poolingObjectList = new List<IPoolableDisposableObject>();
    private List<GameObject> _cacheObs = new List<GameObject>();
    private List<GameObject> _cacheBlock = new List<GameObject>();
    private List<GameObject> _cacheTrap = new List<GameObject>();

    private List<BoxCollider> enableColliders = new List<BoxCollider>();

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

    private void Tiling(BoxCollider[] _BoxColliders)
    {
        ChangePositionOfParentToMatchChildPosition(_BoxColliders[0].transform.parent,
            _BoxColliders[0].transform,
            new Vector3(0, 0, 0));

        centers = _BoxColliders.Select(x => x.center)
                                  .ToArray();
        for (var i = 1; i < _BoxColliders.Length; i++)
        {
            var baseCollider = _BoxColliders[i - 1];
            var alignedCollider = _BoxColliders[i];
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
    private void TilingPaddingBlocks(MasterTrainingBlockComboType masterTrainingBlockComboType)
    {
        if (masterTrainingBlockComboType != MasterTrainingBlockComboType.Modular) return;
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

    private void TilingPaddingBlocks(BoxCollider[] _BoxColliders, BoxCollider _PaddingHeadCollider, BoxCollider _PaddingTailCollider, MasterTrainingBlockComboType masterTrainingBlockComboType)
    {
        if (masterTrainingBlockComboType != MasterTrainingBlockComboType.Modular) return;
        if (!_BoxColliders.Any())
        {
            ChangePositionOfParentToMatchChildPosition(_PaddingHeadCollider.transform.parent,
                _PaddingHeadCollider.transform,
                new Vector3(0, 0, -(0 + _PaddingHeadCollider.bounds.extents.z)));

            ChangePositionOfParentToMatchChildPosition(_PaddingTailCollider.transform.parent,
                _PaddingTailCollider.transform,
                new Vector3(0, 0, (_BoxColliders.Length - 1) * 0 * 2 + (0 + _PaddingHeadCollider.bounds.extents.z)));
        }
        else
        {
            AlignCollider(_BoxColliders.First(), _PaddingHeadCollider, -1);
            AlignCollider(_BoxColliders.Last(), _PaddingTailCollider, 1);
        }
    }

    private void PlaceStartObjectAtOffsetToFirstBlock(float offset)
    {
        if (allPlatformColliders.Count == 0) return;
        var firstCollider = allPlatformColliders.First();
        var boundsExtents = firstCollider.bounds.extents;
        var localPosition = new Vector3(0, boundsExtents.y + firstCollider.center.y, -boundsExtents.z + offset);
        start.transform.position = localPosition + firstCollider.transform.position;
    }
    
    private void PlaceEndObjectAtOffsetToLastBlock(float offset)
    {
        if (allPlatformColliders.Count == 0) return;
        var lastCollider = allPlatformColliders.Last();
        var boundsExtents = lastCollider.bounds.extents;
        var localPosition = new Vector3(0, boundsExtents.y + lastCollider.center.y, boundsExtents.z - offset);
        end.transform.position = localPosition + lastCollider.transform.position;
    }

    private void PlaceStartObjectAtOffsetToFirstBlock(List<BoxCollider> _allPlatformColliders, float offset)
    {
        var firstCollider = _allPlatformColliders.First(); if (firstCollider == null) return;
        var boundsExtents = firstCollider.bounds.extents;
        var localPosition = new Vector3(0, boundsExtents.y + firstCollider.center.y, -boundsExtents.z + offset);
        start.transform.position = localPosition + firstCollider.transform.position;
    }

    private void PlaceEndObjectAtOffsetToLastBlock(List<BoxCollider> _allPlatformColliders, float offset)
    {
        var lastCollider = _allPlatformColliders.Last();
        var boundsExtents = lastCollider.bounds.extents;
        var localPosition = new Vector3(0, boundsExtents.y + lastCollider.center.y, boundsExtents.z - offset);
        end.transform.position = localPosition + lastCollider.transform.position;
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

    private void EnableCollider(List<BoxCollider> colliders)
    {
        colliders.ForEach(x => x.enabled = true);
    }

    public void GenerateBlock(Vector3 startPosition,
                              GameObject[] blockPrefabs,
                              GameObject paddingStartPrefab,
                              GameObject paddingEndPrefab,
                              float jumpingPoint,
                              float landingPoint,
                              MasterTrainingBlockComboType masterTrainingBlockComboType,
                              GameObject[] sceneryObjects,
                              GameObjectPoolList gameObjectPoolList)
    {
        InstantiateBlocks(blockPrefabs, paddingStartPrefab, paddingEndPrefab, masterTrainingBlockComboType);
        Tiling();
        TilingPaddingBlocks(masterTrainingBlockComboType);
        PlaceStartObjectAtOffsetToFirstBlock(landingPoint);
        PlaceEndObjectAtOffsetToLastBlock(jumpingPoint);
        AlignToStartPosition(startPosition);
        GenerateSceneryObjects(sceneryObjects, gameObjectPoolList);
    }

    private IEnumerator GenerateBlockAsync(Vector3 startPosition,
                                           GameObject[] blockPrefabs,
                                           GameObject paddingStartPrefab,
                                           GameObject paddingEndPrefab,
                                           float jumpingPoint,
                                           float landingPoint,
                                           MasterTrainingBlockComboType masterTrainingBlockComboType)
    {
        List<BoxCollider> sss = new List<BoxCollider>();
        enableColliders.Clear();
        var paddingHead = Instantiate_PaddingHeadCollider(paddingStartPrefab, masterTrainingBlockComboType);
        var paddingTail = Instantiate_PaddingTailCollider(paddingEndPrefab, masterTrainingBlockComboType);
        var headCol = paddingHead.GetComponentInChildren<BoxCollider>();
        var tailCol = paddingTail.GetComponentInChildren<BoxCollider>();
        //if (headCol.enabled) { enableColliders.Add(headCol); headCol.enabled = false; }
        //if (tailCol.enabled) { enableColliders.Add(tailCol); tailCol.enabled = false; }
        yield return InstantiateBlocksAsync(blockPrefabs, (s1)=>{
            paddingHeadCollider = headCol;
            paddingTailCollider = tailCol;
            paddingTail.transform.SetAsLastSibling();

            sss.AddRange(s1);
            boxColliders = sss.ToArray();
            allPlatformColliders.Add(paddingHeadCollider);
            allPlatformColliders.AddRange(sss);
            allPlatformColliders.Add(paddingTailCollider);
        });

        Tiling(boxColliders);
        TilingPaddingBlocks(boxColliders, paddingHeadCollider, paddingTailCollider, masterTrainingBlockComboType);
        PlaceStartObjectAtOffsetToFirstBlock(allPlatformColliders, landingPoint);
        PlaceEndObjectAtOffsetToLastBlock(allPlatformColliders, jumpingPoint);
        AlignToStartPosition(startPosition);
        yield return null;
        EnableCollider(enableColliders);
    }

    public void GenerateBlock(Vector3 startPosition,
                              GameObject[] blockPrefabs,
                              GameObject paddingStartPrefab,
                              GameObject paddingEndPrefab,
                              float jumpingPoint,
                              float landingPoint,
                              MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo,
                              float coinRadius,
                              GameObject[] obstaclesPrefab,
                              GameObject[] trapsPrefab,
                              GameObject[] sceneryObjects,
                              GameObjectPoolList gameObjectPoolList)
    {
        IsReady = false; 
        GenerateBlock(startPosition, blockPrefabs, paddingStartPrefab, paddingEndPrefab, jumpingPoint, landingPoint, masterHorseTrainingBlockCombo.MasterTrainingBlockComboType, sceneryObjects, gameObjectPoolList);
        GenerateObstacle(masterHorseTrainingBlockCombo.ObstacleList, obstaclesPrefab);
        GenerateCoins(masterHorseTrainingBlockCombo.CoinList, coinRadius);
        GenerateTraps(masterHorseTrainingBlockCombo.TrapList, trapsPrefab);
        IsReady = true;
    }

    public IEnumerator GenerateBlockAsync(Vector3 startPosition,
                              GameObject[] blockPrefabs,
                              GameObject paddingStartPrefab,
                              GameObject paddingEndPrefab,
                              float jumpingPoint,
                              float landingPoint,
                              MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo,
                              float coinRadius,
                              GameObject[] obstaclesPrefab,
                              GameObject[] trapsPrefab,
                              GameObject[] sceneryObjectPrefabs,
                              PlatformGeneratorPool _pool,
                              GameObjectPoolList gameObjectPoolList)
    {
        pool = _pool;
        IsReady = false;
        //GenerateBlock(startPosition, blockPrefabs, paddingStartPrefab, paddingEndPrefab, jumpingPoint, landingPoint, masterHorseTrainingBlockCombo.MasterTrainingBlockComboType);
        yield return GenerateBlockAsync(startPosition, blockPrefabs, paddingStartPrefab, paddingEndPrefab, jumpingPoint, landingPoint, masterHorseTrainingBlockCombo.MasterTrainingBlockComboType);
        yield return GenerateObstacleAsync(masterHorseTrainingBlockCombo.ObstacleList, obstaclesPrefab);
        GenerateCoins(masterHorseTrainingBlockCombo.CoinList, coinRadius);
        yield return GenerateTrapAsync(masterHorseTrainingBlockCombo.TrapList, trapsPrefab);
        GenerateSceneryObjects(sceneryObjectPrefabs, gameObjectPoolList);
        yield return AlignObstacle(masterHorseTrainingBlockCombo.ObstacleList);
        IsReady = true;
    }

    private void GenerateCoins(Coin[] coinsList, float coinRadius)
    {
        coinsList.ForEach(x =>
        {
            var coin = Instantiate(coinPrefab, this.transform);
            coin.transform.localPosition = x.localPosition.ToVector3();
            coin.Init(x.numberOfCoin, x.benzierPointPositions.Select(x => x.ToVector3()).ToArray(), coinRadius, this.pool);
        });
    }

    private GameObject InstantiateGameObject(GameObjectPoolList gameObjectPoolList,
                                             GameObject prefab)
    {
        var gameObjectWrapper = gameObjectPoolList.Get(prefab);
        poolingObjectList.Add(gameObjectWrapper);
        return gameObjectWrapper.GameObject;
    }

    private void GenerateObstacle(Obstacle[] obstacleList,
                                  GameObject[] obstaclesPrefab)
    {
        obstacleList.ForEach(x =>
        {
            var obstaclesPrefabParent = obstaclesPrefab.FirstOrDefault(saveObstacles => saveObstacles.name == x.type);
            _cacheObs.Add(CreatObstacle(obstaclesPrefabParent, x.localPosition));
        });
    }

    private IEnumerator GenerateObstacleAsync(Obstacle[] obstacleList,
                                  GameObject[] obstaclesPrefab)
    {
        //obstacleList.ForEach(x =>
        //{
        //    var obstaclesPrefabParent = obstaclesPrefab.FirstOrDefault(saveObstacles => saveObstacles.name == x.type);
        //    _cacheObs.Add(CreatObstacle(obstaclesPrefabParent, x.localPosition));
        //});
        int len = obstacleList.Length;
        for (int i = 0; i < len; i++)
        {
            var x = obstacleList[i];
            var obstaclesPrefabParent = obstaclesPrefab.FirstOrDefault(saveObstacles => saveObstacles.name == x.type);
            _cacheObs.Add(CreatObstacle(obstaclesPrefabParent, x.localPosition));
            if (i % 5 == 0) yield return null;
        }
    }

    private IEnumerator AlignObstacle(Obstacle[] obstacleList)
    {
        int len = obstacleList.Length;
        for (int i = 0; i < len; i++)
        {
            var x = obstacleList[i];
            _cacheObs[i].transform.localPosition = x.localPosition.ToVector3();
            if (i % 5 == 0) yield return null;
        }
    }

    private GameObject CreatObstacle(GameObject obstaclesPrefabParent,
                                     Position localPosition)
    {
        var prefab = obstaclesPrefabParent
                     .transform.Cast<Transform>()
                     .Where(x => !x.gameObject.name.Contains("dummy"))
                     .RandomElement();
        var obstacle = Instantiate(prefab.gameObject, transform);//(GameObject)pool.GetOrInstante(prefab.gameObject, transform);
        obstacle.name = prefab.name;
        obstacle.transform.localPosition = localPosition.ToVector3();
        return obstacle;
    }


    private void InstantiateBlocks(GameObject[] gameObjects,
                                   GameObject paddingHead,
                                   GameObject paddingTail,
                                   MasterTrainingBlockComboType trainingBlockComboType)
    {
        if (trainingBlockComboType == MasterTrainingBlockComboType.Modular)
        {
            paddingHeadCollider = Instantiate(paddingHead, this.blockContainer).GetComponentInChildren<BoxCollider>();
            allPlatformColliders.Add(paddingHeadCollider);
        }

        boxColliders = gameObjects.Select(x => Instantiate(x, this.blockContainer).GetComponentInChildren<BoxCollider>())
                                  .ToArray();
        allPlatformColliders.AddRange(boxColliders);
        
        if (trainingBlockComboType == MasterTrainingBlockComboType.Modular)
        {
            paddingTailCollider = Instantiate(paddingTail, this.blockContainer).GetComponentInChildren<BoxCollider>();
            allPlatformColliders.Add(paddingTailCollider);
        }
    }

    private IEnumerator InstantiateBlocksAsync(GameObject[] gameObjects, System.Action<BoxCollider[]> finish)
    {
        var len = gameObjects.Length;
        var BoxColliders = new List<BoxCollider>();
        for (int i = 0; i < len; i++)
        {
            var x = gameObjects[i];
            var ss = Instantiate(x, this.blockContainer).GetComponentInChildren<BoxCollider>();
            //if(ss.enabled)
            //    enableColliders.Add(ss);
            //ss.enabled = false;
            BoxColliders.Add(ss);
            if (i % 5 == 0) yield return null;
        }
        finish?.Invoke(BoxColliders.ToArray());
    }

    private GameObject Instantiate_PaddingHeadCollider(GameObject paddingHead, MasterTrainingBlockComboType trainingBlockComboType)
    {
        if (trainingBlockComboType == MasterTrainingBlockComboType.Modular)
        {
            //if (pool != default)
            //{
            //    var _paddingHead = ((GameObject)pool.GetOrInstante(paddingHead, this.blockContainer));
            //    _cacheBlock.Add(_paddingHead);
            //    return _paddingHead.GetComponentInChildren<BoxCollider>();
            //}
            //else
            {
                return Instantiate(paddingHead, this.blockContainer);
            }
        }
        return null;
    }

    private GameObject Instantiate_PaddingTailCollider(GameObject paddingTail, MasterTrainingBlockComboType trainingBlockComboType)
    {
        if (trainingBlockComboType == MasterTrainingBlockComboType.Modular)
        {
            //if (pool != default)
            //{
            //    var _paddingTail = ((GameObject)pool.GetOrInstante(paddingTail, this.blockContainer));
            //    _cacheBlock.Add(_paddingTail);
            //    return paddingTail.GetComponentInChildren<BoxCollider>();
            //}
            //else
            {
                return Instantiate(paddingTail, this.blockContainer);
            }
        }
        return null;
    }

    public static void Snap(BoxCollider floor, Collider objetToSnap)
    {
        var bounds = floor.bounds;
        var yHeadOffset = floor.center.y + bounds.extents.y;
        var obstacleBounds = objetToSnap.bounds;
        var yObstacleOffset = -obstacleBounds.center.y + obstacleBounds.extents.y;
        objetToSnap.transform.position = floor.transform.position
                                         + Vector3.up * (yHeadOffset + yObstacleOffset);
    }

    public override void Clear()
    {
        if (_cacheObs.Count > 0)
        {
            var destroyObs = _cacheObs.ToArray();
            destroyObs.ForEach(x => { if (this.pool != default) this.pool.AddToPool(x.name, x.gameObject); else Object.Destroy(x); });
            _cacheObs.Clear();
        }
        if (_cacheBlock.Count > 0)
        {
            var destroyObs = _cacheBlock.ToArray();
            destroyObs.ForEach(x => { if (this.pool != default) this.pool.AddToPool(x.name, x.gameObject); else Object.Destroy(x); });
            _cacheBlock.Clear();
        }
        this.pool = default;
        
        poolingObjectList.ForEach(x => x.ReturnToPool());
        poolingObjectList.Clear();
    }
}
