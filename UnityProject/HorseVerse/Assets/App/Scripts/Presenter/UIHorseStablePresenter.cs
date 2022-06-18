using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;

public class UIHorseStablePresenter : IDisposable
{
    private UIHorseStable uiHorseStable = default;
    private CancellationTokenSource cts = default;
    public event Action OnViewHorseDetail = EmptyAction.Instance;
    public event Action OnBack = EmptyAction.Instance;
    private IDIContainer container;

    private IQuickRaceDomainService quickRaceDomainService = default;
    private IReadOnlyHorseRepository horseRepository = default;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();

    public UIHorseStablePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowUIHorseStableAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        uiHorseStable ??= await UILoader.Load<UIHorseStable>(token: cts.Token);

        uiHorseStable.SetEntity(new UIHorseStable.Entity()
        {
            stableHorseAvatarList = new UIComponentHorseStableAvatarList.Entity()
            {
                entities = HorseRepository.Models.Values.Select(x => new UIComponentHorseStableAvatar.Entity()
                {
                    selectBtn = new ButtonComponent.Entity(() => OnSelectHorseAsync(x.MasterHorseId).Forget())
                }).ToArray()
            },
            backBtn = new ButtonComponent.Entity(OnBack)
        });
        await uiHorseStable.In();
    }

    private async UniTaskVoid OnSelectHorseAsync(long masterHorseId)
    {
        await QuickRaceDomainService.ChangeHorse(masterHorseId);
        OnViewHorseDetail.Invoke();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeUnload(ref uiHorseStable);
    }
}
