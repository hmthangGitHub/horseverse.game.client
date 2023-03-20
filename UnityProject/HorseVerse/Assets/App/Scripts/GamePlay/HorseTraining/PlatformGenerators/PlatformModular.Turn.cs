using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlatformModular 
{
    private BlockObjectData GenerateTurn(GameObject turnPrefab)
    {
        var turn = Instantiate(turnPrefab, this.blockContainer);
        var bl = turn.GetComponent<BlockObjectData>();
        allPlatformColliders.AddRange(bl.BoxColliders);
        return bl;
    }

    public IEnumerator GenerateTurnBlockAsync(Vector3 startPosition,
                              GameObject turnPrefab,
                              float jumpingPoint,
                              float landingPoint,
                              GameObject[] sceneryObjectPrefabs,
                              PlatformGeneratorPool _pool,
                              GameObjectPoolList gameObjectPoolList)
    {
        pool = _pool;
        IsReady = false;
        var bl = GenerateTurn(turnPrefab);
        yield return null;
        GenerateSceneryObjects(sceneryObjectPrefabs, gameObjectPoolList);
        IsReady = true;

        PlaceStartObjectFromBlockData(bl);
        PlaceEndObjectFromBlockData(bl);
        AlignToStartPosition(startPosition);
        yield return null;
    }

    private void PlaceStartObjectFromBlockData(BlockObjectData data)
    {
        start.transform.position = data.StartPoint.position;
    }

    private void PlaceEndObjectFromBlockData(BlockObjectData data)
    {
        end.transform.position = data.EndPoint.position;
    }

}
