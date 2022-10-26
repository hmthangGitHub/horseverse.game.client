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
    public float padding;
    public float spacing;
    public Vector2Int blockNumbers;
    public TrainingMapBlock trainingMapBlockPrefab;
    public Action OnJump = ActionUtility.EmptyAction.Instance;

    public void GenerateBlocks(Vector3 relativePointToPlayer, Vector3 lastEndPosition)
    {
        gameObject.SetActive(true);
        var numberOfBlocks = UnityEngine.Random.Range(blockNumbers.x, blockNumbers.y);
        var scale = platform.localScale;
        scale.z = (GetPositionOfBlockIndex(numberOfBlocks - 1) + Vector3.forward * padding * 2).z;
        platform.localScale = scale;
        GenerateBlocks(numberOfBlocks);
        var offset = transform.position - start.position;
        transform.position = relativePointToPlayer + lastEndPosition + offset;
    }

    private void GenerateBlocks(int numberOfBlocks)
    {
        for (int i = 0; i < numberOfBlocks; i++)
        {
            var mapBlock = Instantiate(trainingMapBlockPrefab, blockContainer);
            mapBlock.transform.localPosition = GetPositionOfBlockIndex(i) + Vector3.forward * padding + Vector3.up * 0.5f;
        }
    }

    private Vector3 GetPositionOfBlockIndex(int i)
    {
        return Vector3.forward * (i * trainingMapBlockPrefab.Size + i * spacing);
    }
}
