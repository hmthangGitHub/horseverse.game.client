using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using UnityEngine;

public class UIHorseTrainingPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIHorseTraining uiHorseTraining;
    private CancellationTokenSource cts;
    private HorseDetailEntityFactory horseDetailEntityFactory;
    private IReadOnlyUserDataRepository userDataRepository;
    private HorseSumaryListEntityFactory horseSumaryListEntityFactory;
    private ITrainingDomainService trainingDomainService;
    private HorseRepository horseRepository;
    private MasterHorseContainer masterHorseContainer;
    private UITrainingLeaderBoardPresenter uiTrainingLeaderBoardPresenter;
    public event Action ToTrainingActionState = ActionUtility.EmptyAction.Instance;
    
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactory ??= container.Inject<HorseDetailEntityFactory>();
    private HorseSumaryListEntityFactory HorseSummaryListEntityFactory => horseSumaryListEntityFactory ??= container.Inject<HorseSumaryListEntityFactory>();
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private ITrainingDomainService TrainingDomainService => trainingDomainService ??= container.Inject<ITrainingDomainService>();
    private HorseRepository HorseRepository => horseRepository ??= container.Inject<HorseRepository>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();

    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= container.Inject<UIBackGroundPresenter>();

    private long currentSelectHorseId = -1;
    private int currentMapIndex = 0;
    public UIHorseTrainingPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIHorseTrainingAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await HideBackgroundAsync();
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        uiHorseTraining ??= await UILoader.Instantiate<UIHorseTraining>(token : cts.Token);
        var currentState = UserDataRepository.Current.TraningTimeStamp <= DateTimeOffset.UtcNow.ToUnixTimeSeconds() ? UIComponentTraningState.TraningState.Prepare 
                                                                                                                    : UIComponentTraningState.TraningState.Processing;
        if (currentState == UIComponentTraningState.TraningState.Prepare && UserDataRepository.Current.TraningTimeStamp != 0)
        {
            await TrainingDomainService.OnDoneTraningPeriod(UserDataRepository.Current.CurrentHorseNftId);
        }
        UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;
        var h = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
        currentSelectHorseId = UserDataRepository.Current.CurrentHorseNftId;

        uiHorseTraining.SetEntity(new UIHorseTraining.Entity()
        {
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.CurrentHorseNftId),
            horseRace = new UIComponentHorseRace.Entity() { type = (int)h.HorseType },
            horseSelectSumaryList = HorseSummaryListEntityFactory.InstantiateHorseSelectSumaryListEntity(),
            prepareState = new UIComponentTrainingPrepareState.Entity()
            {
                toTraningBtn = new ButtonComponent.Entity(() => ToTrainingAsync().Forget())
                {
                    isInteractable = HorseRepository.GetHorseDataModel(userDataRepository.Current.CurrentHorseNftId).Happiness 
                                     >= UserSettingLocalRepository.MasterDataModel.TrainingHappinessCost
                },
                traningCost = UserSettingLocalRepository.MasterDataModel.TrainingHappinessCost,
#if ENABLE_DEBUG_MODULE
                mapSelection = new UIComponentHorseTraningMapSelection.Entity()
                {
                    mapToggleGroup = new UIComponentToggleGroup.Entity() { 
                        onActiveToggle = (val) => { if (val)
                            {
                                currentMapIndex = uiHorseTraining.prepareState.mapSelection.mapToggleGroup.toggleGroup.GetFirstActiveToggle().transform.GetSiblingIndex();
                                Debug.Log("currentMapIndex " + currentMapIndex);
                            } 
                        } 
                    }
                },
#endif
            },
            processingState = currentState == UIComponentTraningState.TraningState.Processing ? new UIComponentTraningProcessingState.Entity()
            {
                timer = new UIComponentCountDownTimer.Entity()
                {
                    utcEndTimeStamp = (int)UserDataRepository.Current.TraningTimeStamp,
                    outDatedEvent = () => OnOutDateAsync().Forget()
                }
            } : default,
            traningStates = new UIComponentTraningState.Entity()
            {
                enumEntity = UserDataRepository.Current.TraningTimeStamp <= DateTimeOffset.UtcNow.ToUnixTimeSeconds() ? UIComponentTraningState.TraningState.Prepare : UIComponentTraningState.TraningState.Processing
            },
            leaderBoardBtn = new ButtonComponent.Entity(() =>
            {
                uiTrainingLeaderBoardPresenter ??= new UITrainingLeaderBoardPresenter(this.container);
                uiTrainingLeaderBoardPresenter.ShowLeaderBoardAsync().Forget();
            })
        });
        uiHorseTraining.In().Forget();
    }

    private async UniTask HideBackgroundAsync()
    {
        await UIBackGroundPresenter.HideBackground();
    }

    private async UniTask ToTrainingAsync()
    {
        var data = await TrainingDomainService.StartTrainingData(UserDataRepository.Current.CurrentHorseNftId);
        if (data.ResultCode == 100)
        {
            ToTrainingActionState.Invoke();
            var userHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
            userHorse.HorseRising.Happiness = data.Happiness;
            await HorseRepository.UpdateModelAsync(userHorse.HorseRising);
            var mapID = currentMapIndex == 0 ? 2001 : 2002;
            container.Bind(new HorseTrainingDataContext()
            {
                HorseMeshInformation = new HorseMeshInformation()
                {
                    //horseModelPath = MasterHorseContainer.FromTypeToMasterHorse(userHorse.HorseType).ModelPath,
                    horseModelPath = MasterHorseContainer.MasterHorseIndexer[userHorse.HorseMasterId].ModelPath,
                    //color1 = userHorse.Color1,
                    //color2 = userHorse.Color2,
                    //color3 = userHorse.Color3,
                    //color4 = userHorse.Color4,
                },
                MasterMapId = mapID,
            });
        }
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.before.CurrentHorseNftId != model.after.CurrentHorseNftId)
        {
            uiHorseTraining.SetHorseDetailEntity(HorseDetailEntityFactory.InstantiateHorseDetailEntity(model.after.CurrentHorseNftId));
            var h = HorseRepository.Models[model.after.CurrentHorseNftId];
            uiHorseTraining.SetHorseRaceEntity( new UIComponentHorseRace.Entity() { type = (int)h.HorseType });

            uiHorseTraining.entity.prepareState.toTraningBtn = new ButtonComponent.Entity(() => ToTrainingAsync()
                .Forget())
            {
                isInteractable = HorseRepository.GetHorseDataModel(userDataRepository.Current.CurrentHorseNftId)
                                                .Happiness
                                 >= UserSettingLocalRepository.MasterDataModel.TrainingHappinessCost
            };
            
            uiHorseTraining.prepareState.toTraningBtn.SetEntity(uiHorseTraining.entity.prepareState.toTraningBtn);
            
            OnSelectHorse(model.after.CurrentHorseNftId);
        }

        if (model.before.TraningTimeStamp == 0 && model.after.TraningTimeStamp != 0)
        {
            uiHorseTraining.ChangeState(UIComponentTraningState.TraningState.Processing);
            uiHorseTraining.entity.processingState = new UIComponentTraningProcessingState.Entity()
            {
                timer = new UIComponentCountDownTimer.Entity()
                {
                    utcEndTimeStamp = (int)model.after.TraningTimeStamp,
                    outDatedEvent = () => OnOutDateAsync().Forget()
                }
            };
            uiHorseTraining.processingState.SetEntity(uiHorseTraining.entity.processingState);
        }

        if (model.before.TraningTimeStamp != 0 && model.after.TraningTimeStamp == 0)
        {
            uiHorseTraining.ChangeState(UIComponentTraningState.TraningState.Prepare);
        }
    }

    private async UniTaskVoid OnOutDateAsync()
    {
        await TrainingDomainService.OnDoneTraningPeriod(UserDataRepository.Current.CurrentHorseNftId);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiHorseTraining);
        DisposeUtility.SafeDispose(ref uiTrainingLeaderBoardPresenter);
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
    }

    private void OnSelectHorse(long nftId)
    {
        if (currentSelectHorseId == nftId) return;

        var l = uiHorseTraining.horseSelectSumaryList.instanceList;
        if (currentSelectHorseId > -1)
        {
            var old = l.FirstOrDefault(o => o.entity.horseNFTId == currentSelectHorseId);
            if(old != null)
            {
                old.selectBtn.SetSelected(false);
            }
            currentSelectHorseId = -1;
        }
        var current = l.FirstOrDefault(o => o.entity.horseNFTId == nftId);
        if (current != default)
        {
            current.selectBtn.SetSelected(true);
            currentSelectHorseId = nftId;
        }
    }

    private void UpdateStatusHorse()
    {
        var userHorse = HorseRepository.Models[currentSelectHorseId];
        uiHorseTraining.prepareState.toTraningBtn.SetInteractable(userHorse.Happiness >= UserSettingLocalRepository.MasterDataModel.TrainingHappinessCost);
    }
}
