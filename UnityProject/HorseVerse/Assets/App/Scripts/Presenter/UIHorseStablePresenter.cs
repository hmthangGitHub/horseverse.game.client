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
    public event Action OnViewHorseDetail = ActionUtility.EmptyAction.Instance;
    private IDIContainer container;

    private IQuickRaceDomainService quickRaceDomainService = default;
    private IReadOnlyHorseRepository horseRepository = default;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();
    
    private IUserDataRepository userDataRepository;
    protected IUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IUserDataRepository>();

    private float currentSelectHorseId = -1;

    public UIHorseStablePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowUIHorseStableAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        uiHorseStable ??= await UILoader.Instantiate<UIHorseStable>(token: cts.Token);
        var current = UserDataRepository.Current.CurrentHorseNftId;
        uiHorseStable.SetEntity(new UIHorseStable.Entity()
        {
            stableHorseAvatarList = new UIComponentHorseStableAvatarList.Entity()
            {
                entities = HorseRepository.Models.Select(x => new UIComponentHorseStableAvatar.Entity()
                {
                    horseNFTId = x.Value.HorseNtfId,
                    horseName = x.Value.Name,
                    horseLevel = x.Value.Level,
                    horseRace = new UIComponentHorseRace.Entity() { type = x.Value.Type },
                    selectBtn = new ButtonSelectedComponent.Entity(() => OnSelectHorseAsync(x.Key).Forget(), x.Value.HorseNtfId == current)
                }).ToArray()
            },
            horseDetail = new UIComponentHorseDetail.Entity()
            {
                horseName = HorseRepository.Models[current].Name,
                level = HorseRepository.Models[current].Level,
                powerProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = HorseRepository.Models[current].PowerBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = HorseRepository.Models[current].PowerRatio
                    }
                },
                speedProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = HorseRepository.Models[current].SpeedBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = HorseRepository.Models[current].SpeedRatio
                    }
                },
                technicallyProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = HorseRepository.Models[current].TechnicallyBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = HorseRepository.Models[current].TechnicallyRatio
                    }
                }
            },
            horseRace = new UIComponentHorseRace.Entity()
            {
                type = HorseRepository.Models[current].Type
            },
            breedingBtn = new ButtonComponent.Entity(()=> OnBreedingAsync().Forget(), false),
            upgradeBtn = new ButtonComponent.Entity(() => OnUpgradeAsync().Forget(), false),
        });
        currentSelectHorseId = current;
        await uiHorseStable.In();
        await UniTask.Delay(500);
        uiHorseStable.breedingBtn.SetInteractable(false);
        uiHorseStable.upgradeBtn.SetInteractable(false);
    }

    private async UniTaskVoid OnSelectHorseAsync(long masterHorseId)
    {
        if (currentSelectHorseId == masterHorseId) return;
        var l = uiHorseStable.stableHorseAvatarList.instanceList;
        if (currentSelectHorseId > -1)
        {
            var old = l.FirstOrDefault(o => o.entity.horseNFTId == currentSelectHorseId);
            if (old != null)
            {
                old.selectBtn.SetSelected(false);
            }
            currentSelectHorseId = -1;
        }
        var current = l.FirstOrDefault(o => o.entity.horseNFTId == masterHorseId);
        if (current != default)
        {
            current.selectBtn.SetSelected(true);
            currentSelectHorseId = masterHorseId;
        }

        var eh = uiHorseStable.horseDetail.entity;
        {
            eh.horseName = HorseRepository.Models[masterHorseId].Name;
            eh.level = HorseRepository.Models[masterHorseId].Level;
            eh.powerProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = HorseRepository.Models[masterHorseId].PowerBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = HorseRepository.Models[masterHorseId].PowerRatio
                }
            };
            eh.speedProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = HorseRepository.Models[masterHorseId].SpeedBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = HorseRepository.Models[masterHorseId].SpeedRatio
                }
            };
            eh.technicallyProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = HorseRepository.Models[masterHorseId].TechnicallyBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = HorseRepository.Models[masterHorseId].TechnicallyRatio
                }
            };
        };

        var er = uiHorseStable.horseRace.entity;
        er.type = HorseRepository.Models[masterHorseId].Type;

        uiHorseStable.SetHorseDetailEntity(eh);
        uiHorseStable.SetHorseRaceEntity(er);

        await QuickRaceDomainService.ChangeHorse(masterHorseId);
        //await OutAsync();
        //OnViewHorseDetail.Invoke();
    }

    private async UniTaskVoid OnBreedingAsync() 
    {
        await UniTask.CompletedTask;
    }

    private async UniTaskVoid OnUpgradeAsync()
    {
        await OutAsync();
        OnViewHorseDetail.Invoke();
        await UniTask.CompletedTask;
    }

    public UniTask OutAsync()
    {
        return uiHorseStable?.Out() ?? UniTask.CompletedTask;
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiHorseStable);
    }
}
