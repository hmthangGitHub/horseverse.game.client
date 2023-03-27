using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformGeneratorModularBlockV2 : PlatformGeneratorBase
{
    private const string TrainingBlockSettingPath = "Maps/MapSettings/training_block_settings";

    private TrainingBlockSettings trainingBlockSettings;
    private CancellationTokenSource cts;
    private MasterTrainingModularBlockContainer masterTrainingModularBlockContainer;
    private GameObjectPoolList gameObjectPoolList = new GameObjectPoolList();

    private string _path = "";

    protected override async UniTask InitializeInternal()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        trainingBlockSettings = await PrimitiveAssetLoader.LoadAssetAsync<TrainingBlockSettings>(TrainingBlockSettingPath, cts.Token);
        masterTrainingModularBlockContainer = await MasterLoader.LoadMasterAsync<MasterTrainingModularBlockContainer>(cts.Token);
    }

    protected override async UniTask InitializeInternal(string path)
    {
        _path = path;
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        trainingBlockSettings = await PrimitiveAssetLoader.LoadAssetAsync<TrainingBlockSettings>($"{TrainingBlockSettingPath}_{path}", cts.Token);
        masterTrainingModularBlockContainer = await MasterLoader.LoadMasterAsync<MasterTrainingModularBlockContainer>(path, cts.Token);
    }

    protected override async UniTask ReleaseInternal()
    {
        PrimitiveAssetLoader.UnloadAssetAtPath($"{TrainingBlockSettingPath}_{_path}");
        MasterLoader.Unload<MasterTrainingModularBlockContainer>(_path);
        _path = "";
        await UniTask.CompletedTask;
    }

    protected override PlatformBase CreatePlatform(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        var randomBlockCombo = GetRandomBlockCombo();

        var paddingStartBlockId= masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdStart);
        var paddingEndBlockId= masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdEnd);

        var modularBlockIds = randomBlockCombo.MasterHorseTrainingBlockIdList; 
        var platform = Instantiate(platformPrefab, this.transform);
        platform.GetComponent<PlatformModular>().GenerateBlock(relativePointToPlayer + lastEndPosition, 
            modularBlockIds.Select(x => trainingBlockSettings.BlocksLookUpTable[x].gameObject).ToArray(), 
            trainingBlockSettings.BlocksLookUpTable[paddingStartBlockId].gameObject,
            trainingBlockSettings.BlocksLookUpTable[paddingEndBlockId].gameObject,
            masterHorseTrainingProperty.JumpingPoint,
            masterHorseTrainingProperty.LandingPoint,
            randomBlockCombo,
            masterHorseTrainingProperty.CoinColliderRadius,
            trainingBlockSettings.obstacles,
            trainingBlockSettings.traps,
            trainingBlockSettings.sceneryObjects,
            gameObjectPoolList);
        
#if ENABLE_DEBUG_MODULE
        platform.GetComponent<PlatformModular>().SetBlockName(randomBlockCombo.Name);
#endif
        return platform;
    }

    protected override async UniTask<PlatformBase> CreatePlatformAsync(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        var randomBlockCombo = GetRandomBlockCombo();

        var paddingStartBlockId = masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdStart);
        var paddingEndBlockId = masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdEnd);

        var modularBlockIds = randomBlockCombo.MasterHorseTrainingBlockIdList;
        var platform = Instantiate(platformPrefab, this.transform);
        var platformModular = platform.GetComponent<PlatformModular>();
        await platformModular.GenerateBlockAsync(relativePointToPlayer + lastEndPosition, 
            modularBlockIds.Select(x => trainingBlockSettings.BlocksLookUpTable[x].gameObject).ToArray(),
            trainingBlockSettings.BlocksLookUpTable[paddingStartBlockId].gameObject,
            trainingBlockSettings.BlocksLookUpTable[paddingEndBlockId].gameObject,
            masterHorseTrainingProperty.JumpingPoint,
            masterHorseTrainingProperty.LandingPoint,
            randomBlockCombo,
            masterHorseTrainingProperty.CoinColliderRadius,
            trainingBlockSettings.obstacles,
            trainingBlockSettings.traps,
            trainingBlockSettings.sceneryObjects,
            pool,
            gameObjectPoolList);
#if ENABLE_DEBUG_MODULE
        platform.GetComponent<PlatformModular>().SetBlockName(randomBlockCombo.Name);
#endif
        return platform;
    }

    protected override async UniTask<PlatformBase> CreatePlatformWithoutSceneryObjectAsync(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        var randomBlockCombo = GetRandomBlockCombo();

        var paddingStartBlockId = masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdStart);
        var paddingEndBlockId = masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdEnd);

        var modularBlockIds = randomBlockCombo.MasterHorseTrainingBlockIdList;
        var platform = Instantiate(platformPrefab, this.transform);
        var platformModular = platform.GetComponent<PlatformModular>();
        await platformModular.GenerateBlockWithouSceneryObjectAsync(relativePointToPlayer + lastEndPosition,
            modularBlockIds.Select(x => trainingBlockSettings.BlocksLookUpTable[x].gameObject).ToArray(),
            trainingBlockSettings.BlocksLookUpTable[paddingStartBlockId].gameObject,
            trainingBlockSettings.BlocksLookUpTable[paddingEndBlockId].gameObject,
            masterHorseTrainingProperty.JumpingPoint,
            masterHorseTrainingProperty.LandingPoint,
            randomBlockCombo,
            masterHorseTrainingProperty.CoinColliderRadius,
            trainingBlockSettings.obstacles,
            trainingBlockSettings.traps,
            pool);
#if ENABLE_DEBUG_MODULE
        platform.GetComponent<PlatformModular>().SetBlockName(randomBlockCombo.Name);
#endif
        return platform;
    }

    protected override async UniTask<PlatformBase> CreateTurnPlatformAsync(TYPE_OF_BLOCK type, Vector3 relativePointToPlayer, Vector3 lastEndPosition)
    {
        var platform = Instantiate(platformPrefab, this.transform);
        var platformModular = platform.GetComponent<PlatformModular>();
        GameObject randomTurnBlock = default;
        if (type == TYPE_OF_BLOCK.TURN_LEFT)
            randomTurnBlock = trainingBlockSettings.turnLeftBlocks.GetRandom();
        else if (type == TYPE_OF_BLOCK.TURN_RIGHT)
            randomTurnBlock = trainingBlockSettings.turnRightBlocks.GetRandom();
        else
            return platform;

        var randomStartBlock = trainingBlockSettings.startBlocks.GetRandom();
        var randomEndBlock = trainingBlockSettings.endBlocks.GetRandom();
        await platformModular.GenerateTurnBlockAsync(relativePointToPlayer + lastEndPosition,
            randomTurnBlock,
            randomStartBlock,
            randomEndBlock,
            masterHorseTrainingProperty.JumpingPoint,
            masterHorseTrainingProperty.LandingPoint,
            trainingBlockSettings.sceneryObjects,
            pool,
            gameObjectPoolList);
#if ENABLE_DEBUG_MODULE
        platform.GetComponent<PlatformModular>().SetBlockName("Turn Block " + randomTurnBlock.name);
#endif
        return platform;
    }

    protected override async UniTask CreateSceneryObectAsync(PlatformBase platform, BoxCollider[] boxColliders)
    {
        var platformModular = platform.GetComponent<PlatformModular>();
        await platformModular.GenerateSceneryObjects(boxColliders, trainingBlockSettings.sceneryObjects, gameObjectPoolList);
    }

    private MasterHorseTrainingBlockCombo GetRandomBlockCombo()
    {
        var masterHorseTrainingBlockGroupId = GetMasterHorseTrainingBlockGroupId();
        return masterHorseTrainingBlockComboContainer.MasterHorseTrainingBlockComboGroupIdIndexer[masterHorseTrainingBlockGroupId]
                                                     .RandomElement();
        //return masterHorseTrainingBlockComboContainer.MasterHorseTrainingBlockComboGroupIdIndexer[masterHorseTrainingBlockGroupId].Where(x => x.Traps.Length > 0).RandomElement();
    }

    private int GetMasterHorseTrainingBlockGroupId()
    {
        var maxWeight = masterTrainingBlockDistributeContainer
                        .DifficultyIndexer[horseTrainingControllerV2.CurrentDifficulty]
                        .Sum(x => x.Weight);
        var randomWeighted = Random.Range(0f, maxWeight);
        var weight = 0;
        var group = 0;
        foreach (var item in masterTrainingBlockDistributeContainer
                     .DifficultyIndexer[horseTrainingControllerV2.CurrentDifficulty])
        {
            weight += item.Weight;
            if (randomWeighted <= weight)
                return item.MasterHorseTrainingBlockGroupId;
        }

        throw new Exception($"Can not get random MasterHorseTrainingBlockGroupId ");
    }

    private string GetPaddingMasterTrainingModularBlockId(string masterTrainingModularBlockId)
    {
        return string.IsNullOrEmpty(masterTrainingModularBlockId) 
            ? masterTrainingModularBlockContainer.MasterTrainingModularBlockIndexer[masterTrainingModularBlockId].MasterTrainingModularBlockId
            : masterTrainingModularBlockContainer.MasterTrainingModularBlockIndexer.Values
                                                 .First(x => x.MasterTrainingModularBlockType == MasterTrainingModularBlockType.Padding)
                                                 .MasterTrainingModularBlockId;
    }
    
    public override void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        trainingBlockSettings = default;
        PrimitiveAssetLoader.UnloadAssetAtPath(TrainingBlockSettingPath);
        MasterLoader.Unload<MasterTrainingModularBlockContainer>();
        if(!string.IsNullOrEmpty(_path)) MasterLoader.Unload<MasterTrainingModularBlockContainer>(_path);
        DisposeUtility.SafeDispose(ref gameObjectPoolList);
    }
}
