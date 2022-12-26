using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
        var randomBlockCombo = masterHorseTrainingBlockComboContainer.MasterHorseTrainingBlockComboIndexer.Values.RandomElement();
        var paddingBlockId = masterTrainingModularBlockContainer.MasterTrainingModularBlockIndexer.Values
                                                                .First(x => x.MasterTrainingModularBlockType == MasterTrainingModularBlockType.Padding)
                                                                .MasterTrainingModularBlockId;

        var modularBlockIds = randomBlockCombo.MasterHorseTrainingBlockIdList; 
        var platform = Instantiate(platformPrefab, this.transform);
        platform.GetComponent<PlatformModular>().GenerateBlock(relativePointToPlayer + lastEndPosition, modularBlockIds.Select(x => trainingBlockSettings.BlocksLookUpTable[x].gameObject).ToArray(), 
            trainingBlockSettings.BlocksLookUpTable[paddingBlockId].gameObject,
            trainingBlockSettings.BlocksLookUpTable[paddingBlockId].gameObject,
            masterHorseTrainingProperty.JumpingPoint,
            masterHorseTrainingProperty.LandingPoint,
            randomBlockCombo,
            trainingBlockSettings.obstacles);
        return platform;
    }
    
    public override void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        trainingBlockSettings = default;
        PrimitiveAssetLoader.UnloadAssetAtPath(TrainingBlockSettingPath);
        MasterLoader.Unload<MasterTrainingModularBlockContainer>();
    }
}
