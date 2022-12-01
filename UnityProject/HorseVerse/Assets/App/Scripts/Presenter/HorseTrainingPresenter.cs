using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private MasterHorseTrainingPropertyContainer masterHorseTrainingPropertyContainer;
    private MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer;
    private MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer;
    private UITrainingCoinCounting uiTrainingCoinCounting;
    private UITrainingPressAnyKey uiTrainingPressAnyKey;
    
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
        masterHorseTrainingPropertyContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingPropertyContainer>(token: cts.Token);
        masterHorseTrainingBlockContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockContainer>(token: cts.Token);
        masterHorseTrainingBlockComboContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockComboContainer>(token: cts.Token);
        
        mapSceneAsset = await SceneAssetLoader.LoadSceneAsync(masterMapContainer.MasterMapIndexer[HorseTrainingDataContext.MasterMapId]
            .MapPath, true, token: cts.Token);

        uiTrainingCoinCounting = await UILoader.Instantiate<UITrainingCoinCounting>(token: cts.Token);
        uiTrainingPressAnyKey = await UILoader.Instantiate<UITrainingPressAnyKey>(token: cts.Token);
        
        await horseTrainingManager.Initialize(
            masterMapContainer.MasterMapIndexer[HorseTrainingDataContext.MasterMapId].MapPath,
            OnTakeCoin,
            () => OnTouchObstacleAsync().Forget(), 
            masterHorseTrainingPropertyContainer.DataList.First(),
            masterHorseTrainingBlockContainer, 
            masterHorseTrainingBlockComboContainer, horseTrainingDataContext.HorseMeshInformation);
        
        uiTrainingPressAnyKey.SetEntity(new UITrainingPressAnyKey.Entity()
        {
            onInput = () =>
            {
                uiTrainingCoinCounting.SetEntity(new UITrainingCoinCounting.Entity()
                {
                    coin = numberOfCoinTaken
                });
                uiTrainingCoinCounting.In().Forget();
                uiTrainingPressAnyKey.Out().Forget();
                horseTrainingManager.StartGame();
            }
        });
    }

    public async UniTask<int> StartTrainingAsync()
    {
        await UniTask.Delay(1500);
        uiTrainingPressAnyKey.In().Forget();
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
        ucs.TrySetResult();
        await UniTask.CompletedTask;
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        
        if (mapSceneAsset != default)
        {
            SceneAssetLoader.UnloadAssetAtPath(masterMapContainer.MasterMapIndexer[HorseTrainingDataContext.MasterMapId].MapPath);
            mapSceneAsset = default;
        }
        UILoader.SafeRelease(ref uiTrainingCoinCounting);
        UILoader.SafeRelease(ref uiTrainingPressAnyKey);
        MasterLoader.SafeRelease(ref masterMapContainer);
        MasterLoader.SafeRelease(ref masterHorseContainer);
        MasterLoader.SafeRelease(ref masterHorseTrainingPropertyContainer);
        MasterLoader.SafeRelease(ref masterHorseTrainingBlockContainer);
        MasterLoader.SafeRelease(ref masterHorseTrainingBlockComboContainer);
        horseTrainingDataContext = default;
        DisposeUtility.SafeDispose(ref horseTrainingManager);
    }
}
