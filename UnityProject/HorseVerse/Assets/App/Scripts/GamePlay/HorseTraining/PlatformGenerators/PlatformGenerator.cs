using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformGenerator : PlatformGeneratorBase
{
    protected override UniTask InitializeInternal()
    {
        return UniTask.FromResult(UniTask.CompletedTask);
    }

    protected override UniTask InitializeInternal(string path)
    {
        return UniTask.FromResult(UniTask.CompletedTask);
    }

    protected override PlatformBase CreatePlatform(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        var platform = Instantiate(platformPrefab, this.transform);
        var platformTest = platform.GetComponent<Platform>();
        platformTest.GenerateBlocks(relativePointToPlayer,
           lastEndPosition,
           masterHorseTrainingBlockContainer,
           masterHorseTrainingBlockComboContainer.MasterHorseTrainingBlockComboIndexer.RandomElement()
                                                 .Value, masterHorseTrainingProperty);
        return platformTest;
    }

    protected override async UniTask<PlatformBase> CreatePlatformAsync(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        return CreatePlatform(relativePointToPlayer, lastEndPosition);
    }

    private void CreateDebugSphere(Vector3 position)
    {
#if UNITY_EDITOR
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.1f;
        sphere.GetComponent<Collider>().enabled = false;
#endif
    }

    public override void Dispose()
    {
    }
}