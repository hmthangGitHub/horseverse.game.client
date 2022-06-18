using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIHorseDetailPresenter : IDisposable
{
    private UIHorseDetail uiHorseDetail = default;
    private CancellationTokenSource cts;
    private IDIContainer container;
    public event Action OnBack = EmptyAction.Instance;

    private HorseDetailEntityFactory horseDetailEntityFactor = default;
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactor ??= container.Inject<HorseDetailEntityFactory>();
    private IReadOnlyUserDataRepository userDataRepository = default;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IHorseDetailDomainService horseDetailDomainService = default;
    private IHorseDetailDomainService HorseDetailDomainService => horseDetailDomainService ??= container.Inject<IHorseDetailDomainService>();
    private IHorseRepository horseRepository = default;
    private IHorseRepository HorseRepository => horseRepository ??= container.Inject<IHorseRepository>();

    public UIHorseDetailPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowUIHorseDetailAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiHorseDetail ??= await UILoader.Load<UIHorseDetail>(token: cts.Token);
        HorseRepository.OnModelUpdate += HorseRepositoryOnModelUpdate;
        uiHorseDetail.SetEntity(new UIHorseDetail.Entity()
        {
            backBtn = new ButtonComponent.Entity(OnBack),
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.MasterHorseId),
            levelUpBtn = new ButtonComponent.Entity(() => OnLevelUpAsync().Forget())
        });
        await uiHorseDetail.In();
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
        UILoader.SafeUnload(ref uiHorseDetail);
    }
}
