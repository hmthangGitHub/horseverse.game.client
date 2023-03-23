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
        return bl;
    }

    public IEnumerator GenerateTurnBlockAsync(Vector3 startPosition,
                              GameObject turnPrefab,
                              GameObject paddingStartPrefab,
                              GameObject paddingEndPrefab,
                              float jumpingPoint,
                              float landingPoint,
                              GameObject[] sceneryObjectPrefabs,
                              PlatformGeneratorPool _pool,
                              GameObjectPoolList gameObjectPoolList)
    {
        pool = _pool;
        IsReady = false;

        var paddingHead = Instantiate_PaddingHeadCollider(paddingStartPrefab, MasterTrainingBlockComboType.Modular);
        var paddingTail = Instantiate_PaddingTailCollider(paddingEndPrefab, MasterTrainingBlockComboType.Modular);
        var headCol = paddingHead.GetComponentInChildren<BoxCollider>();
        var tailCol = paddingTail.GetComponentInChildren<BoxCollider>();

        var bl = GenerateTurn(turnPrefab);
        yield return null;

        paddingHeadCollider = headCol;
        paddingTailCollider = tailCol;
        paddingTail.transform.SetAsLastSibling();
        allPlatformColliders.Add(paddingHeadCollider);
        allPlatformColliders.AddRange(bl.BoxColliders);
        allPlatformColliders.Add(paddingTailCollider);

        ChangePositionOfParentToMatchChildPosition(paddingHeadCollider.transform.parent,
            paddingHeadCollider.transform,
            new Vector3(0, 0, -(0 + PaddingHeadCollider.bounds.extents.z)));

        var worldPos = paddingHeadCollider.transform.position + paddingHeadCollider.center +
                       new Vector3(0, paddingHeadCollider.bounds.extents.y, 1 * paddingHeadCollider.bounds.extents.z);
        bl.transform.position = worldPos;

        paddingTailCollider.transform.parent.position = bl.EndPoint.position;
        paddingTailCollider.transform.parent.forward = bl.EndPoint.forward;

        


        //GenerateSceneryObjects(sceneryObjectPrefabs, gameObjectPoolList, 1);
        IsReady = true;

        PlaceStartObjectFromBlockData(bl);
        PlaceEndObjectFromBlockData(bl);
        AlignToStartPosition(startPosition);
        bl.Init();
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
