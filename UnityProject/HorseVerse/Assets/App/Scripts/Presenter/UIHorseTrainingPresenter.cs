using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIHorseTrainingPresenter : IDisposable
{
    public Action OnBack = EmptyAction.Instance;
    public UIHorseTraining uiHorseTraining = default;
    private CancellationTokenSource cts;
    private HorseDetailEntityFactory horseDetailEntityFactory;
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactory ??= container.Inject<HorseDetailEntityFactory>();
    private HorseSumaryListEntityFactory horseSumaryListEntityFactory;
    private HorseSumaryListEntityFactory HorseSumaryListEntityFactory => horseSumaryListEntityFactory ??= container.Inject<HorseSumaryListEntityFactory>();
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IDIContainer container;

    private ITrainingDomainService trainingDomainService;
    private ITrainingDomainService TrainingDomainService => trainingDomainService ??= container.Inject<ITrainingDomainService>();
    private IReadOnlyHorseRepository horseRepository;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();

    private const int traningCost = 100;

    public UIHorseTrainingPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIHorseTraningAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        uiHorseTraining ??= await UILoader.Load<UIHorseTraining>(token : cts.Token);
        var currentState = UserDataRepository.Current.TraningTimeStamp <= DateTimeOffset.UtcNow.ToUnixTimeSeconds() ? UIComponentTraningState.TraningState.Prepare 
                                                                                                                    : UIComponentTraningState.TraningState.Processing;
        if (currentState == UIComponentTraningState.TraningState.Prepare && UserDataRepository.Current.TraningTimeStamp != 0)
        {
            await TrainingDomainService.OnDoneTraningPeriod(UserDataRepository.Current.MasterHorseId);
        }
        UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;

        uiHorseTraining.SetEntity(new UIHorseTraining.Entity()
        {
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.MasterHorseId),
            horseSelectSumaryList = HorseSumaryListEntityFactory.InstantiateHorseSelectSumaryListEntity(),
            prepareState = new UIComponentTrainingPrepareState.Entity()
            {
                mapSelection = new UIComponentHorseTraningMapSelection.Entity()
                {
                    mapToggleGroup = new UIComponentToggleGroup.Entity()
                    {
                    },
                },
                toTraningBtn = new ButtonComponent.Entity(() => ToTraningAsycn().Forget()),
                traningCost = traningCost,
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
            backBtn = new ButtonComponent.Entity(OnBack)
        });
        uiHorseTraining.In().Forget();
    }

    private async UniTask ToTraningAsycn()
    {
        await TrainingDomainService.SendHorseToTraining(UserDataRepository.Current.MasterHorseId);
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.before.MasterHorseId != model.after.MasterHorseId)
        {
            uiHorseTraining.entity.horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(model.after.MasterHorseId);
            uiHorseTraining.horseDetail.SetEntity(uiHorseTraining.entity.horseDetail);
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
        await TrainingDomainService.OnDoneTraningPeriod(UserDataRepository.Current.MasterHorseId);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeUnload(ref uiHorseTraining);
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
    }
}
