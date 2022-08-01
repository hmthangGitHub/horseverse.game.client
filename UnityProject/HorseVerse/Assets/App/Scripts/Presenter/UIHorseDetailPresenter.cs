using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class UIHorseDetailPresenter : IDisposable
{
    private UIHorseDetail uiHorseDetail;
    private CancellationTokenSource cts;
    private readonly IDIContainer container;

    private HorseDetailEntityFactory horseDetailEntityFactor;
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactor ??= container.Inject<HorseDetailEntityFactory>();
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IHorseDetailDomainService horseDetailDomainService;
    private IHorseDetailDomainService HorseDetailDomainService => horseDetailDomainService ??= container.Inject<IHorseDetailDomainService>();
    private IHorseRepository horseRepository;
    private IHorseRepository HorseRepository => horseRepository ??= container.Inject<IHorseRepository>();

    public UIHorseDetailPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowUIHorseDetailAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiHorseDetail ??= await UILoader.Instantiate<UIHorseDetail>(token: cts.Token);
        HorseRepository.OnModelUpdate += HorseRepositoryOnModelUpdate;
        uiHorseDetail.SetEntity(new UIHorseDetail.Entity()
        {
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.MasterHorseId),
            levelUpBtn = new ButtonComponent.Entity(() => OnLevelUpAsync().Forget())
        });
        await uiHorseDetail.In();
    }

    public UniTask OutAsync()
    {
        return uiHorseDetail?.Out() ?? UniTask.CompletedTask;
    }

    private void HorseRepositoryOnModelUpdate((HorseDataModel before, HorseDataModel after) model)
    {
        uiHorseDetail.entity.horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.MasterHorseId);
        uiHorseDetail.horseDetail.SetEntity(uiHorseDetail.entity.horseDetail);
    }

    private async UniTaskVoid OnLevelUpAsync()
    {
        await HorseDetailDomainService.LevelUp(UserDataRepository.Current.MasterHorseId);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        HorseRepository.OnModelUpdate -= HorseRepositoryOnModelUpdate;
        UILoader.SafeRelease(ref uiHorseDetail);
    }
}
