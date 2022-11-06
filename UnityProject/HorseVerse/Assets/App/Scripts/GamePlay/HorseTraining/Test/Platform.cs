using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Platform : MonoBehaviour
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

    private void GenerateBlock(MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer, MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, float blockSpacing, float blockPadding)
    {
        for (int i = 0; i < masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList.Length; i++)
        {
            var mapBlock = Instantiate(trainingMapBlockPrefab, blockContainer);
            mapBlock.transform.localPosition = GetPositionOfBlockIndex(i, blockSpacing) + Vector3.forward * blockPadding + Vector3.up * 0.5f;
            mapBlock.Lanes.ForEach((x, laneIndex) =>
            {
                var masterHorseTrainingBlockId = masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList[i];
                x.GenBlockLane(masterHorseTrainingBlockContainer.MasterHorseTrainingBlockIndexer[masterHorseTrainingBlockId][laneIndex]);
            });
        }
    }

    public void GenerateBlocks(Vector3 relativePointToPlayer, Vector3 lastEndPosition, MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer, MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, MasterHorseTrainingProperty masterHorseTrainingProperty)
    {
        // start.transform.localPosition = -Vector3.forward * masterHorseTrainingProperty.LandingPoint;
        // end.transform.localPosition = Vector3.forward * masterHorseTrainingProperty.JumpingPoint;
        gameObject.SetActive(true);
        var numberOfBlocks = masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList.Length;
        var scale = platform.localScale;
        scale.z = (GetPositionOfBlockIndex(numberOfBlocks - 1, masterHorseTrainingProperty.BlockSpacing) + Vector3.forward * masterHorseTrainingProperty.BlockPadding * 2).z;
        platform.localScale = scale;
        var hafScale = scale.z *0.5f;
        start.transform.localPosition = -Vector3.forward * (hafScale - masterHorseTrainingProperty.LandingPoint) * 0.5f / hafScale;
        end.transform.localPosition = Vector3.forward * (hafScale - masterHorseTrainingProperty.JumpingPoint) * 0.5f/ hafScale;
        GenerateBlock(masterHorseTrainingBlockContainer, masterHorseTrainingBlockCombo, masterHorseTrainingProperty.BlockSpacing, masterHorseTrainingProperty.BlockPadding);
        var offset = transform.position - start.position;
        transform.position = relativePointToPlayer + lastEndPosition + offset;
    }
}
