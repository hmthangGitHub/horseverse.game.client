#if UNITY_EDITOR
using UnityEngine;

public partial class PlatformModular
{
    public GameObject[] testBlocks;
    public GameObject testPaddingHead;
    public GameObject testPaddingTail;
    public float testOffset;
    public Transform testStartPosition;
    public MasterTrainingBlockComboType masterTrainingBlockComboTypeTest;

    [ContextMenu("PlaceStartObjectAtOffsetToFirstBlock")]
    public void PlaceStartObjectAtOffsetToFirstBlock()
    {
        PlaceStartObjectAtOffsetToFirstBlock(testOffset);
    }

    [ContextMenu("PlaceEndObjectAtOffsetToLastBlock")]
    public void PlaceEndObjectAtOffsetToLastBlock()
    {
        PlaceEndObjectAtOffsetToLastBlock(testOffset);
    }
    
    [ContextMenu("AlignToStartPosition")]
    public void AlignToStartPosition()
    {
        AlignToStartPosition(testStartPosition.position);
    }
    
    [ContextMenu("GenerateBlock")]
    public void GenerateBlock()
    {
        GenerateBlock(testStartPosition.position, testBlocks, testPaddingHead, testPaddingTail, testOffset, testOffset, masterTrainingBlockComboTypeTest);
    }
}
#endif