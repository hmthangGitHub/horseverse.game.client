using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;

public class UIHorseTrainingPresenter : IDisposable
{
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
    public event Action ToTraningActionState = ActionUtility.EmptyAction.Instance;

    public UIHorseTrainingPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIHorseTraningAsync()
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

        uiHorseTraining.SetEntity(new UIHorseTraining.Entity()
        {
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.CurrentHorseNftId),
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
        });
        uiHorseTraining.In().Forget();
    }

    private async UniTask ToTraningAsycn()
    {
        ToTraningActionState.Invoke();
        container.Bind(new HorseTrainingDataContext()
        {
            masterHorseId = 10000001,
            masterMapId = 10001003,
        });
        //await TrainingDomainService.SendHorseToTraining(UserDataRepository.Current.MasterHorseId);
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.before.CurrentHorseNftId != model.after.CurrentHorseNftId)
        {
            uiHorseTraining.SetHorseDetailEntity(HorseDetailEntityFactory.InstantiateHorseDetailEntity(model.after.CurrentHorseNftId));
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
}
