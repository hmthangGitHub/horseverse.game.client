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
    private IReadOnlyHorseRepository horseRepository;
    private MasterHorseContainer masterHorseContainer;
    private const int trainingCost = 100;
    public event Action ToTrainingActionState = ActionUtility.EmptyAction.Instance;
    
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactory ??= container.Inject<HorseDetailEntityFactory>();
    private HorseSumaryListEntityFactory HorseSummaryListEntityFactory => horseSumaryListEntityFactory ??= container.Inject<HorseSumaryListEntityFactory>();
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private ITrainingDomainService TrainingDomainService => trainingDomainService ??= container.Inject<ITrainingDomainService>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();

    private long currentSelectHorseId = -1;
    public UIHorseTrainingPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIHorseTrainingAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
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
            horseRace = new UIComponentHorseRace.Entity() { type = h.Type },
            horseSelectSumaryList = HorseSummaryListEntityFactory.InstantiateHorseSelectSumaryListEntity(OnSelectHorse),
            prepareState = new UIComponentTrainingPrepareState.Entity()
            {
                //mapSelection = new UIComponentHorseTraningMapSelection.Entity()
                //{
                //    mapToggleGroup = new UIComponentToggleGroup.Entity()
                //    {
                //    },
                //},
                toTraningBtn = new ButtonComponent.Entity(() => ToTrainingAsync().Forget()),
                traningCost = trainingCost,
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
        });
        uiHorseTraining.In().Forget();
    }

    private async UniTask ToTrainingAsync()
    {
        ToTrainingActionState.Invoke();
        
        var userHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
        
        container.Bind(new HorseTrainingDataContext()
        {
            HorseMeshInformation = new HorseMeshInformation()
            {
                horseModelPath = MasterHorseContainer.MasterHorseIndexer[userHorse.MasterHorseId].ModelPath,
                color1 = userHorse.Color1,
                color2 = userHorse.Color2,
                color3 = userHorse.Color3,
                color4 = userHorse.Color4,
            },
            MasterMapId = 10001003,
        });
        //await TrainingDomainService.SendHorseToTraining(UserDataRepository.Current.MasterHorseId);
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.before.CurrentHorseNftId != model.after.CurrentHorseNftId)
        {
            uiHorseTraining.SetHorseDetailEntity(HorseDetailEntityFactory.InstantiateHorseDetailEntity(model.after.CurrentHorseNftId));
            var h = HorseRepository.Models[model.after.CurrentHorseNftId];
            uiHorseTraining.SetHorseRaceEntity( new UIComponentHorseRace.Entity() { type = h.Type });
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
}
