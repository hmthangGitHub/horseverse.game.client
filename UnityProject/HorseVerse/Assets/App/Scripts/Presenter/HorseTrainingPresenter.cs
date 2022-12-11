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
    private UniTaskCompletionSource<bool> trainingUcsRetry;

    private HorseTrainingDataContext HorseTrainingDataContext => horseTrainingDataContext ??= Container.Inject<HorseTrainingDataContext>();
    private ITrainingDomainService TrainingDomainService => trainingDomainService ??= Container.Inject<ITrainingDomainService>();

    public HorseTrainingPresenter(IDIContainer container)
    {
        Container = container;
        cts = new CancellationTokenSource();
    }

    public async UniTask LoadAssetsAsync()
    {
        masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>(token: cts.Token);
        mapSceneAsset = await SceneAssetLoader.LoadSceneAsync(masterMapContainer.MasterMapIndexer[HorseTrainingDataContext.MasterMapId]
            .MapPath, true, token: cts.Token);
        horseTrainingManager ??= Object.Instantiate((await Resources.LoadAsync<HorseTrainingManager>("GamePlay/HorseTrainingManager") as HorseTrainingManager));
        
        masterHorseContainer = await MasterLoader.LoadMasterAsync<MasterHorseContainer>(token: cts.Token);
        masterHorseTrainingPropertyContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingPropertyContainer>(token: cts.Token);
        masterHorseTrainingBlockContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockContainer>(token: cts.Token);
        masterHorseTrainingBlockComboContainer = await MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockComboContainer>(token: cts.Token);

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
                    btnSetting = new ButtonComponent.Entity(UniTask.Action(async () => await OnBtnPauseClicked()))
                });
                uiTrainingCoinCounting.In().Forget();
                uiTrainingPressAnyKey.Out().Forget();
                horseTrainingManager.StartGame();
                AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunTraining);
            }
        });
    }

    public async UniTask<bool> StartTrainingAsync()
    {
        await UniTask.Delay(1500);
        SoundController.PlayMusicTrainingInGame();
        uiTrainingPressAnyKey.In().Forget();
        trainingUcsRetry = new UniTaskCompletionSource<bool>();
        var isNeedRetry =  await trainingUcsRetry.Task.AttachExternalCancellation(cts.Token);
        AudioManager.Instance.StopSound();
        return isNeedRetry;
    }

    private async UniTask OnBtnPauseClicked()
    {
        Time.timeScale = 0.0f;
        var uiConfirm = await UILoader.Instantiate<UIPopUpPause>(token: cts.Token);
        var exitBtnEntity = new ButtonComponent.Entity(UniTask.Action(async() =>
        {
            await uiConfirm.Out();
            if (await AskForQuit())
            {
                await uiConfirm.Out();
                UILoader.SafeRelease(ref uiConfirm);
                trainingUcsRetry.TrySetResult(false);
                SoundController.PlayMusicBase();
            }
            else
            {
                await uiConfirm.In();
            }
        }));
        uiConfirm.SetEntity(new UIPopUpPause.Entity()
        {
            settingBtn = exitBtnEntity,
            continueBtn = new ButtonComponent.Entity(UniTask.Action(async() =>
            {
                await uiConfirm.Out();
                UILoader.SafeRelease(ref uiConfirm);
                Time.timeScale = 1.0f;
            })),
            exitBtn = exitBtnEntity,
        });
        uiConfirm.In().Forget();
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

    private async UniTask<bool> AskForQuit()
    {
        var askForQuitUcs = new UniTaskCompletionSource<bool>();
        var uiConfirm = await UILoader.Instantiate<UIPopupYesNoMessage>(token: cts.Token);
        uiConfirm.SetEntity(new UIPopupYesNoMessage.Entity()
        {
            title = "NOTICE",
            message = "Do you want to exit ? You won't receive any reward once you do.",
            yesBtn = new ButtonComponent.Entity(() =>
            {
                askForQuitUcs.TrySetResult(true);
            }),
            noBtn = new ButtonComponent.Entity(() => { askForQuitUcs.TrySetResult(false); })
        });
        await uiConfirm.In();
        var result = await askForQuitUcs.Task.AttachExternalCancellation(cts.Token);
        UILoader.SafeRelease(ref uiConfirm);
        return result;
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
        Time.timeScale = 0.0f;
        AudioManager.Instance.StopSound();
        SoundController.PlayHitObstance();
        var data = await TrainingDomainService.GetTrainingRewardData(distanceOfRunning, numberOfCoinTaken);
        if (data.ResultCode == 100) {
            await ShowUIHorseTrainingResultAsync(data);
        }
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        Time.timeScale = 1.0f;
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
        AudioManager.Instance.StopSound();
    }

    public async UniTask ShowUIHorseTrainingResultAsync(io.hverse.game.protogen.TrainingRewardsResponse result)
    {
        var popup = await UILoader.Instantiate<UITrainingResult>(token: cts.Token);
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
            confirmBtn = new ButtonComponent.Entity(() =>
            {
                trainingUcsRetry.TrySetResult(false);
                UILoader.SafeRelease(ref popup);
            }),
            retryBtn = new ButtonComponent.Entity(() =>
            {
                trainingUcsRetry.TrySetResult(true);
                UILoader.SafeRelease(ref popup);
            }),
            boxReward = new UITrainingResultRewardComponent.Entity() { Total = (int)numbox},
            coinReward = new UITrainingResultRewardComponent.Entity() { Total = (int)numcoin },
            currentEnergy = 1,
            totalEnergy = 10,
            score = result.PointNumber,
        });
        await popup.In();
    }
}
