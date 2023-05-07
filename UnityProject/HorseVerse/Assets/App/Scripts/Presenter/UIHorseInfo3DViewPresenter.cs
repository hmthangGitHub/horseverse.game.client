using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIHorseInfo3DViewPresenter : IDisposable
{
    private IDIContainer container = default;
    private IReadOnlyHorseRepository horseRepository = null;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();

    private UIHorseInfo3DView uiHorse3DView = default;
    private CancellationTokenSource cts = default;

    private MasterHorseContainer masterHorseContainer = default;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private bool isIn = false;
    public UIHorseInfo3DViewPresenter(IDIContainer container)
    {
        this.container = container;    
    }

    public async UniTask ShowHorse3DViewAsync(long MasterHorseId)
    {
        Debug.Log("Load master Horse ID " + MasterHorseId);
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        uiHorse3DView ??= await UILoader.Instantiate<UIHorseInfo3DView>(token: cts.Token);
        if (!isIn)
        {
            uiHorse3DView.SetEntity(new UIHorseInfo3DView.Entity()
            {
                horseLoader = new HorseLoader.Entity()
                {
                    horse = MasterHorseContainer.MasterHorseIndexer[MasterHorseId].ModelPath,
                }
            });
            uiHorse3DView.transform.SetAsFirstSibling();
            uiHorse3DView.In().Forget();
            isIn = true;
        }
    }

    public async UniTask ShowHorse3DViewAsync(long MasterHorseId, Color color1, Color color2, Color color3, Color color4)
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        uiHorse3DView ??= await UILoader.Instantiate<UIHorseInfo3DView>(token: cts.Token);
        if (!isIn)
        {
            uiHorse3DView.SetEntity(new UIHorseInfo3DView.Entity()
            {
                horseLoader = new HorseLoader.Entity()
                {
                    horse = MasterHorseContainer.MasterHorseIndexer[MasterHorseId].ModelPath,
                    color1 = color1,
                    color2 = color2,
                    color3 = color3,
                    color4 = color4,
                }
            });
            uiHorse3DView.transform.SetAsFirstSibling();
            uiHorse3DView.In().Forget();
            isIn = true;
        }
    }

    public async UniTask HideHorse3DViewAsync()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        isIn = false;
        await GetOutTask();
    }

    private UniTask GetOutTask()
    {
        return uiHorse3DView?.Out() ?? UniTask.CompletedTask;
    }

    public async UniTask UpdateMode(long MasterHorseId)
    {
        var masterHorse = MasterHorseContainer.MasterHorseIndexer[MasterHorseId];
        uiHorse3DView.entity.horseLoader = new HorseLoader.Entity()
        {
            horse = masterHorse.ModelPath,
        };
        uiHorse3DView.horseLoader.SetEntity(uiHorse3DView.entity.horseLoader);
        await uiHorse3DView.In();
    }

    public async UniTask UpdateMode(long MasterHorseId, Color color1, Color color2, Color color3, Color color4)
    {
        var masterHorse = MasterHorseContainer.MasterHorseIndexer[MasterHorseId];
        uiHorse3DView.entity.horseLoader = new HorseLoader.Entity()
        {
            horse = masterHorse.ModelPath,
            color1 = color1,
            color2 = color2,
            color3 = color3,
            color4 = color4,
        };
        uiHorse3DView.horseLoader.SetEntity(uiHorse3DView.entity.horseLoader);
        await uiHorse3DView.In();
    }

    public void Dispose()
    {
        HideHorse3DViewAsync().Forget();
        UILoader.SafeRelease(ref uiHorse3DView);
    }
}
