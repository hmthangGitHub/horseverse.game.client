using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformGeneratorModularBlock : PlatformGeneratorBase
{
    private const string TrainingBlockSettingPath = "Maps/MapSettings/training_block_settings";
    private TrainingBlockSettings trainingBlockSettings;
    private CancellationTokenSource cts;
    private MasterTrainingModularBlockContainer masterTrainingModularBlockContainer;

    protected override async UniTask InitializeInternal()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        trainingBlockSettings = await PrimitiveAssetLoader.LoadAssetAsync<TrainingBlockSettings>(TrainingBlockSettingPath, cts.Token);
        masterTrainingModularBlockContainer = await MasterLoader.LoadMasterAsync<MasterTrainingModularBlockContainer>(cts.Token);
    }

    protected override PlatformBase CreatePlatform(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        var randomBlockCombo = GetRandomBlockCombo();

        var paddingStartBlockId= masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdStart);
        var paddingEndBlockId= masterTrainingModularBlockContainer.GetFirstPaddingIfEmpty(randomBlockCombo.MasterTrainingModularBlockIdEnd);

        var modularBlockIds = randomBlockCombo.MasterHorseTrainingBlockIdList; 
        var platform = Instantiate(platformPrefab, this.transform);
        StartCoroutine(platform.GetComponent<PlatformModular>().GenerateBlockAsync(relativePointToPlayer + lastEndPosition, modularBlockIds.Select(x => trainingBlockSettings.BlocksLookUpTable[x].gameObject).ToArray(), 
            trainingBlockSettings.BlocksLookUpTable[paddingStartBlockId].gameObject,
            trainingBlockSettings.BlocksLookUpTable[paddingEndBlockId].gameObject,
            masterHorseTrainingProperty.JumpingPoint,
            masterHorseTrainingProperty.LandingPoint,
            randomBlockCombo,
            masterHorseTrainingProperty.CoinColliderRadius,
            trainingBlockSettings.obstacles,
            pool));
        return platform;
    }

    private MasterHorseTrainingBlockCombo GetRandomBlockCombo()
    {
        var masterHorseTrainingBlockGroupId = GetMasterHorseTrainingBlockGroupId();
        return masterHorseTrainingBlockComboContainer.MasterHorseTrainingBlockComboGroupIdIndexer[masterHorseTrainingBlockGroupId]
                                                     .RandomElement();
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
    }
}
