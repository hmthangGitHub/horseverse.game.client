#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public partial class PlatformModular
{
    public GameObject[] testBlocks;
    private GameObject[] sceneryObjectsTest;
    private GameObjectPoolList gameObjectPoolTest;
    public GameObject testPaddingHead;
    public GameObject testPaddingTail;
    public float testOffset;
    public Transform testStartPosition;
    public MasterTrainingBlockComboType masterTrainingBlockComboTypeTest;

    [ContextMenu("PlaceStartObjectAtOffsetToFirstBlock")]
    public void PlaceStartObjectAtOffsetToFirstBlockTest()
    {
        PlaceStartObjectAtOffsetToFirstBlock(testOffset);
    }

    [ContextMenu("PlaceEndObjectAtOffsetToLastBlock")]
    public void PlaceEndObjectAtOffsetToLastBlockTest()
    {
        PlaceEndObjectAtOffsetToLastBlock(testOffset);
    }
    
    [ContextMenu("AlignToStartPosition")]
    public void AlignToStartPositionTest()
    {
        AlignToStartPosition(testStartPosition.position);
    }
    
    [ContextMenu("GenerateBlock")]
    public void GenerateBlockTest()
    {
        allPlatformColliders.ForEach(x =>
        {
            DestroyImmediate(x.transform.parent.gameObject);
        });
        allPlatformColliders.Clear();
        gameObjectPoolTest?.Dispose();
        gameObjectPoolTest ??= new GameObjectPoolList();
        sceneryObjectsTest = AssetDatabase.LoadAssetAtPath<TrainingBlockSettings>("Assets/App/AssetBundles/Maps/MapSettings/training_block_settings.asset")
                                            .sceneryObjects;
        if (sceneryBoxContainer != default)
        {
            DestroyImmediate(sceneryBoxContainer.gameObject);
        }
        
        if (sceneryConflictRegion != default)
        {
            DestroyImmediate(sceneryConflictRegion.gameObject);
        }
        
        GenerateBlock(testStartPosition.position, testBlocks, testPaddingHead, testPaddingTail, testOffset, testOffset, masterTrainingBlockComboTypeTest, sceneryObjectsTest, gameObjectPoolTest);
    #if ENABLE_DEBUG_MODULE
        this.SetBlockName("BLOCK SOME THING I DON KNOW");
    #endif
    }
    
    [ContextMenu("CreateSceneryContainer")]
    public void CreateSceneryContainerTest()
    {
        if (sceneryBoxContainer != default)
        {
            DestroyImmediate(sceneryBoxContainer.gameObject);
        }
        
        if (sceneryConflictRegion != default)
        {
            DestroyImmediate(sceneryConflictRegion.gameObject);
        }
        CreateSceneryRegions();
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            gameObjectPoolTest?.Dispose();
            sceneryObjectsTest = Array.Empty<GameObject>();
        }
    }
}
#endif