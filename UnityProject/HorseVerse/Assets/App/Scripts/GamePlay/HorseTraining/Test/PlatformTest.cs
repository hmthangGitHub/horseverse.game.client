using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlatformTest : MonoBehaviour
{
    public Transform platform;
    public Transform start;
    public Transform end;
    public Transform blockContainer;
    public TrainingMapBlock trainingMapBlockPrefab;
    public Action OnJump = ActionUtility.EmptyAction.Instance;

    private void GenerateBlocks(int numberOfBlocks, float blockSpacing, float blockPadding)
    {
        for (int i = 0; i < numberOfBlocks; i++)
        {
            var mapBlock = Instantiate(trainingMapBlockPrefab, blockContainer);
            mapBlock.transform.localPosition = GetPositionOfBlockIndex(i, blockSpacing) + Vector3.forward * blockPadding + Vector3.up * 0.5f;
        }
    }

    private Vector3 GetPositionOfBlockIndex(int i, float spacing)
    {
        return Vector3.forward * (i * trainingMapBlockPrefab.Size + i * spacing);
    }

    public void GenerateBlocks(Vector3 relativePointToPlayer, Vector3 lastEndPosition, float blockPadding, float blockSpacing, int blockNumbersMin, int blockNumbersMax, float jumpPoint, float landingPoint)
    {
        start.transform.localPosition = -Vector3.forward * landingPoint;
        end.transform.localPosition = Vector3.forward * jumpPoint;
        gameObject.SetActive(true);
        var numberOfBlocks = UnityEngine.Random.Range(blockNumbersMin, blockNumbersMax);
        var scale = platform.localScale;
        scale.z = (GetPositionOfBlockIndex(numberOfBlocks - 1, blockSpacing) + Vector3.forward * blockPadding * 2).z;
        platform.localScale = scale;
        GenerateBlocks(numberOfBlocks, blockSpacing, blockPadding);
        var offset = transform.position - start.position;
        transform.position = relativePointToPlayer + lastEndPosition + offset;
    }
}
