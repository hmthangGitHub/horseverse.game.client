using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class HorseTrainingPresenter : IDisposable
{
    private IDIContainer Container { get; }
    private HorseTrainingDataContext horseTrainingDataContext;
    private HorseTrainingManager horseTrainingManager;
    private MasterHorseContainer masterHorseContainer;
    private MasterMapContainer masterMapContainer;
    private UITrainingCoinCounting uiTrainingCoinCounting;
    
    private CancellationTokenSource cts;
    private Scene mapSceneAsset;
    private int numberOfCoinTaken = 0;
    private UniTaskCompletionSource ucs = new UniTaskCompletionSource();
    
    private HorseTrainingDataContext HorseTrainingDataContext => horseTrainingDataContext ??= Container.Inject<HorseTrainingDataContext>();

    public HorseTrainingPresenter(IDIContainer container)
    {
        Container = container;
        cts = new CancellationTokenSource();
    }

    public async UniTask LoadAssetsAsync()
    {
        horseTrainingManager ??= Object.Instantiate((await Resources.LoadAsync<HorseTrainingManager>("GamePlay/HorseTrainingManager") as HorseTrainingManager));
        
        masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>(token: cts.Token);
        masterHorseContainer = await MasterLoader.LoadMasterAsync<MasterHorseContainer>(token: cts.Token);
        
        mapSceneAsset = await SceneAssetLoader.LoadSceneAsync(masterMapContainer.MasterMapIndexer[HorseTrainingDataContext.masterMapId]
            .MapPath, true, token: cts.Token);

        uiTrainingCoinCounting = await UILoader.Instantiate<UITrainingCoinCounting>(token: cts.Token);
        
        await horseTrainingManager.Initialize(
            masterHorseContainer.MasterHorseIndexer[HorseTrainingDataContext.masterHorseId].ModelPath,
            masterMapContainer.MasterMapIndexer[HorseTrainingDataContext.masterMapId].MapPath,
            OnTakeCoin,
            () => OnTouchObstacleAsync().Forget());
        
        uiTrainingCoinCounting.SetEntity(new UITrainingCoinCounting.Entity()
        {
            coin = numberOfCoinTaken
        });
        uiTrainingCoinCounting.In().Forget();
    }

    public async UniTask<int> StartTrainingAsync()
    {
        ucs = new UniTaskCompletionSource();
        await ucs.Task.AttachExternalCancellation(cts.Token);
        return numberOfCoinTaken;
    }

    private void OnTakeCoin()
    {
        numberOfCoinTaken++;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        uiTrainingCoinCounting.coin.SetEntity(numberOfCoinTaken);
    }

    private async UniTaskVoid OnTouchObstacleAsync()
    {
        await uiTrainingCoinCounting.Out();
        ucs.TrySetResult();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        
        if (mapSceneAsset != default)
        {
            SceneAssetLoader.UnloadAssetAtPath(masterMapContainer.MasterMapIndexer[HorseTrainingDataContext.masterMapId].MapPath);
            mapSceneAsset = default;
        }
        UILoader.SafeRelease(ref uiTrainingCoinCounting);
        MasterLoader.SafeRelease(ref masterMapContainer);
        MasterLoader.SafeRelease(ref masterHorseContainer);
        Object.Destroy(horseTrainingManager?.gameObject);
        
        horseTrainingManager = default;
        horseTrainingDataContext = default;
    }
}
