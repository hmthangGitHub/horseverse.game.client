﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlatformGeneratorV2 : PlatformGeneratorBase
{
    private const string TrainingBlockSettingPath = "Maps/MapSettings/training_block_settings";
    private TrainingBlockSettings trainingBlockSettings;
    private CancellationTokenSource cts;

    protected override async UniTask InitializeInternal()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        trainingBlockSettings = await PrimitiveAssetLoader.LoadAssetAsync<TrainingBlockSettings>(TrainingBlockSettingPath, cts.Token);
        await UniTask.CompletedTask;
    }

    public override void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        trainingBlockSettings = default;
        PrimitiveAssetLoader.UnloadAssetAtPath(TrainingBlockSettingPath);
    }

    protected override PlatformBase CreatePlatform(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        var platform = Instantiate(platformPrefab, this.transform);
        platform.GetComponent<PlatformV2>().GenerateBlocks(relativePointToPlayer, lastEndPosition,
            trainingBlockSettings.blockCombos.RandomElement(), trainingBlockSettings.obstacles,
            trainingBlockSettings.sceneryObjects,
            masterHorseTrainingProperty);
        return platform;
    }
}