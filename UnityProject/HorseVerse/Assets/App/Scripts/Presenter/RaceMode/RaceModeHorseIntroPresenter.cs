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

    public async UniTask ShowHorsesInfoIntroAsync(IHorseBriefInfo[] horseBriefInfo, Vector3 horsePosition, Quaternion rotation)
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        this.horsePosition = horsePosition;
        this.rotation = rotation;

        try
        {
            for (int i = 0; i < horseBriefInfo.Length; i++)
            {
                await ShowHorseInfoAsync(horseBriefInfo[i],
                                         i + 1,
                                         5.0f)
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

    private async UniTask ShowHorseInfoAsync(IHorseBriefInfo horseRaceInfo, int gate, float introTime)
    {
        var ucs = new UniTaskCompletionSource();
        void OnSkipHorse()
        {
            ucs.TrySetResult();
        }

        uiHorseInfoIntro.SetEntity(new UIHorseInfoIntro.Entity()
        {
            gate = gate,
            horseInfo = new UIComponentHorseDetail.Entity()
            {
                horseName = horseRaceInfo.Name,
                level = horseRaceInfo.Level,
                powerProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = horseRaceInfo.PowerBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = horseRaceInfo.PowerRatio
                    }
                },
                speedProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = horseRaceInfo.SpeedBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = horseRaceInfo.SpeedRatio
                    }
                },
                technicallyProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = horseRaceInfo.TechnicallyBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = horseRaceInfo.TechnicallyRatio
                    }
                },
            },
            outerBtn = new ButtonComponent.Entity(OnSkipHorse),
            skipAllBtn = new ButtonComponent.Entity(OnSkipAllHorseIntro)
        });

        var horseMeshInformation = MasterHorseContainer.GetHorseMeshInformation(horseRaceInfo.MeshInformation, HorseModelMode.Intro);
        if (uiHorse3DIntro.entity == default)
        {
            uiHorse3DIntro.SetEntity(new UIHorse3DInRaceSceneIntro.Entity()
            {
                horseModelLoader = new UIHorseModelLoader.Entity()
                {
                    horse = horseMeshInformation.horseModelPath,
                    color1 = horseMeshInformation.color1,
                    color2 = horseMeshInformation.color2,
                    color3 = horseMeshInformation.color3,
                    color4 = horseMeshInformation.color4,
                    position = horsePosition,
                    rotation = rotation
                }
            });
            uiHorse3DIntro.In().Forget();
        }
        else
        {
            uiHorse3DIntro.entity.horseModelLoader = new UIHorseModelLoader.Entity()
            {
                horse = horseMeshInformation.horseModelPath,
                color1 = horseMeshInformation.color1,
                color2 = horseMeshInformation.color2,
                color3 = horseMeshInformation.color3,
                color4 = horseMeshInformation.color4,
                position = horsePosition,
                rotation = rotation
            };
            uiHorse3DIntro.horseModelLoader.SetEntity(uiHorse3DIntro.entity.horseModelLoader);
        }
        
        uiHorseInfoIntro.In().Forget();

        await UniTask.WhenAny(ucs.Task.AttachExternalCancellation(cts.Token), 
            UniTask.Delay(TimeSpan.FromSeconds(introTime), cancellationToken: cts.Token));
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