using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using io.hverse.game.protogen;

public class UIHorseDetailPresenter : IDisposable
{
    private UIHorseStableDetail uiHorseStableDetail;
    private CancellationTokenSource cts;
    private readonly IDIContainer container;

    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IHorseDetailDomainService horseDetailDomainService;
    private IHorseDetailDomainService HorseDetailDomainService => horseDetailDomainService ??= container.Inject<IHorseDetailDomainService>();
    private IHorseRepository horseRepository;
    private IHorseRepository HorseRepository => horseRepository ??= container.Inject<IHorseRepository>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter = default;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= container.Inject<UIHorse3DViewPresenter>();

    public UIHorseDetailPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowUIHorseDetailAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiHorseStableDetail ??= await UILoader.Instantiate<UIHorseStableDetail>(token: cts.Token);
        HorseRepository.OnModelUpdate += HorseRepositoryOnModelUpdate;
        UserDataRepository.OnModelUpdate += OnUserModelUpdate;
        SetHorseInfo();
    }

    private void OnUserModelUpdate((UserDataModel before, UserDataModel after) obj)
    {
        if (obj.after.CurrentHorseNftId != obj.before.CurrentHorseNftId)
        {
            SetHorseInfo();
        }
    }

    private UIHorseStableDetail.Entity CreateCurrentHorseDetailInfoEntity()
    {
        var currentHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
        return new UIHorseStableDetail.Entity()
        {
            tab = new UIComponentHorseStableDetailTab.Entity()
            {
                initialValue = UIComponentHorseStableDetailTab.Tab.Info,
            },
            info = new UIComponentHorseDetailInfo.Entity()
            {
                acceleration = 100,
                endurance = 100,
                speed = 100,
                breedCount = 100,
                coinCollected = 100,
                sprintEnergy = 100,
                sprintNumber = 100,
                sprintRegen = 100,
                sprintSpeed = 100,
                sprintTime = 100,
                breedCountMax = 100,
                coinCollectedMax = 100
            },
            briefInfo = new UIHorseStableBriefInfo.Entity()
            {
                horseName = currentHorse.Name,
                age = 1,
                element = currentHorse.Type.ConvertTo<HorseType, UIComponentHorseElement.Element>(),
                sex = UIHorseSexInfo.Sex.Female,
                leftBtn = new ButtonComponent.Entity(() =>
                {
                    UIHorse3DViewPresenter.ChangeHorseOnSwipe(-1).Forget();
                }),
                rightBtn = new ButtonComponent.Entity(() =>
                {
                    UIHorse3DViewPresenter.ChangeHorseOnSwipe(1).Forget();
                }),
            }
        };
    }

    public UniTask OutAsync()
    {
        return uiHorseStableDetail?.Out() ?? UniTask.CompletedTask;
    }

    private void HorseRepositoryOnModelUpdate((HorseDataModel before, HorseDataModel after) model)
    {
        SetHorseInfo();
    }

    private void SetHorseInfo()
    {
        uiHorseStableDetail.SetEntity(CreateCurrentHorseDetailInfoEntity());
        uiHorseStableDetail.In()
                           .Forget();
    }

    private async UniTaskVoid OnLevelUpAsync()
    {
        await HorseDetailDomainService.LevelUp(UserDataRepository.Current.CurrentHorseNftId);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        HorseRepository.OnModelUpdate -= HorseRepositoryOnModelUpdate;
        UserDataRepository.OnModelUpdate -= OnUserModelUpdate;
        UILoader.SafeRelease(ref uiHorseStableDetail);
    }
}
