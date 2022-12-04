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

    private ITrainingDomainService trainingDomainService;

    private CancellationTokenSource cts;
    private Scene mapSceneAsset;
    private int numberOfCoinTaken = 0;
    private int distanceOfRunning = 0;
    private UniTaskCompletionSource ucs = new UniTaskCompletionSource();

    private HorseTrainingDataContext HorseTrainingDataContext => horseTrainingDataContext ??= Container.Inject<HorseTrainingDataContext>();
    private ITrainingDomainService TrainingDomainService => trainingDomainService ??= Container.Inject<ITrainingDomainService>();

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
                    coin = numberOfCoinTaken,
                    btnSetting = new ButtonComponent.Entity(UniTask.Action(async () => { await OnBtnPauseClicked(); }))
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

    private async UniTask OnBtnPauseClicked()
    {
        var uiConfirm = await UILoader.Instantiate<UIPopUpPause>(token: cts.Token);
        bool wait = true;
        int type = 0;
        uiConfirm.SetEntity(new UIPopUpPause.Entity()
        {
            settingBtn = new ButtonComponent.Entity(() => { wait = false; type = 1; }),
            continueBtn = new ButtonComponent.Entity(() => { wait = false; }),
            exitBtn = new ButtonComponent.Entity(() => { wait = false; type = 2; })
        });
        await uiConfirm.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref uiConfirm);
        
        if (type == 1)
        {
            await OnBtnSettingClicked();
        }
        else if (type == 2)
        {
            await OnBtnExitClicked();
        }
    }

    private async UniTask OnBtnSettingClicked()
    {
        var uiConfirm = await UILoader.Instantiate<UIPopupYesNoMessage>(token: cts.Token);
        bool wait = true;
        uiConfirm.SetEntity(new UIPopupYesNoMessage.Entity()
        {
            title = "NOTICE",
            message = "Do you want to exit ? You won't receive any reward once you do.",
            yesBtn = new ButtonComponent.Entity(() => { wait = false; }),
            noBtn = new ButtonComponent.Entity(() => { wait = false; })
        });
        await uiConfirm.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref uiConfirm);
    }

    private async UniTask OnBtnExitClicked()
    {
        var uiConfirm = await UILoader.Instantiate<UIPopupYesNoMessage>(token: cts.Token);
        bool wait = true;
        uiConfirm.SetEntity(new UIPopupYesNoMessage.Entity()
        {
            title = "NOTICE",
            message = "Do you want to exit ? You won't receive any reward once you do.",
            yesBtn = new ButtonComponent.Entity(() => { wait = false; }),
            noBtn = new ButtonComponent.Entity(() => { wait = false; })
        });
        await uiConfirm.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref uiConfirm);
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
        var data = await TrainingDomainService.GetTrainingRewardData(distanceOfRunning, numberOfCoinTaken);
        if (data.ResultCode == 100) {
            await ShowUIHorseTrainingResultAsync(data);
        }
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

    public async UniTask ShowUIHorseTrainingResultAsync(io.hverse.game.protogen.TrainingRewardsResponse result)
    {
        var popup = await UILoader.Instantiate<UITrainingResult>(token: cts.Token);
        bool wait = true;
        long numbox = 0;
        long numcoin = 0;

        foreach(var item in result.Rewards)
        {
            if (item.Type == io.hverse.game.protogen.RewardType.Chip)
                numcoin += item.Amount;
            if (item.Type == io.hverse.game.protogen.RewardType.Chest)
                numbox += item.Amount;
        }

        popup.SetEntity(new UITrainingResult.Entity()
        {
            confirmBtn = new ButtonComponent.Entity(() => { wait = false; }),
            retryBtn = new ButtonComponent.Entity(() => { wait = false; }),
            boxReward = new UITrainingResultRewardComponent.Entity() { Total = (int)numbox},
            coinReward = new UITrainingResultRewardComponent.Entity() { Total = (int)numcoin },
            currentEnergy = 1,
            totalEnergy = 10,
            score = result.PointNumber,
        });
        await popup.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref popup);
    }
}
