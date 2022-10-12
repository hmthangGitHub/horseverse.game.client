using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RaceModeHorseIntroPresenter : IDisposable
{
    private UIHorseInfoIntro uiHorseInfoIntro;
    private UIHorse3DInRaceSceneIntro uiHorse3DIntro;
    private CancellationTokenSource cts;
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private Vector3 horsePosition;
    private Quaternion rotation;
    private IDIContainer Container { get; }

    public RaceModeHorseIntroPresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask ShowHorsesInfoIntroAsync(long[] masterHorseIds, Vector3 horsePosition, Quaternion rotation)
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        this.horsePosition = horsePosition;
        this.rotation = rotation;

        try
        {
            for (int i = 0; i < masterHorseIds.Length; i++)
            {
                await ShowHorseInfoAsync(MasterHorseContainer.MasterHorseIndexer[masterHorseIds[i]],
                                         i + 1,
                                         100.0f)
                     .AttachExternalCancellation(cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async UniTask LoadUIAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiHorseInfoIntro = await UILoader.Instantiate<UIHorseInfoIntro>(token: cts.Token);
        uiHorse3DIntro = await UILoader.Instantiate<UIHorse3DInRaceSceneIntro>(token: cts.Token);
    }

    private async UniTask ShowHorseInfoAsync(MasterHorse masterHorse, int gate, float introTime)
    {
        var ucs = new UniTaskCompletionSource();
        void OnSkipHorse()
        {
            ucs.TrySetResult();
        }

        uiHorseInfoIntro.SetEntity(new UIHorseInfoIntro.Entity()
        {
            gate = gate,
            horseName = masterHorse.Name,
            outerBtn = new ButtonComponent.Entity(OnSkipHorse),
            skipAllBtn = new ButtonComponent.Entity(OnSkipAllHorseIntro)
        });

        if (uiHorse3DIntro.entity == default)
        {
            uiHorse3DIntro.SetEntity(new UIHorse3DInRaceSceneIntro.Entity()
            {
                horseModelLoader = new UIHorseModelLoader.Entity()
                {
                    horse = masterHorse.IntroRaceModeModelPath,
                    position = horsePosition,
                    rotation = rotation
                }
            });
            uiHorse3DIntro.In().Forget();
        }
        else
        {
            uiHorse3DIntro.entity.horseModelLoader.horse = masterHorse.IntroRaceModeModelPath;
            uiHorse3DIntro.horseModelLoader.SetEntity(uiHorse3DIntro.entity.horseModelLoader);
        }
        
        uiHorseInfoIntro.In().Forget();

        await UniTask.WhenAny(ucs.Task, UniTask.Delay((int)(introTime * 1000)));
    }

    private void OnSkipAllHorseIntro()
    {
        cts.SafeCancelAndDispose();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        masterHorseContainer = default;
        UILoader.SafeRelease(ref uiHorseInfoIntro);
        UILoader.SafeRelease(ref uiHorse3DIntro);
    }
} 