using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseSumaryListEntityFactory
{
    private IDIContainer container;
    private IReadOnlyHorseRepository horseRepository;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private IReadOnlyUserDataRepository userReponsitory;
    private IReadOnlyUserDataRepository UserReponsitory => userReponsitory??= container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private IQuickRaceDomainService quickRaceDomainService;
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();
    public HorseSumaryListEntityFactory(IDIContainer container)
    {
        this.container = container;
    }

    public UIComponentTraningHorseSelectSumaryList.Entity InstantiateHorseSelectSumaryListEntity()
    {
        var current = UserReponsitory.Current.CurrentHorseNftId;
        return new UIComponentTraningHorseSelectSumaryList.Entity()
        {
            entities = HorseRepository.Models.Select(x => new UIComponentTraningHorseSelectSumary.Entity()
            {
                horseNFTId = x.Value.HorseNtfId,
                horseName = x.Value.Name,
                horseLevel = x.Value.Level,
                horseRace = new UIComponentHorseRace.Entity() {type = (int)x.Value.Type },
                selectBtn = new ButtonSelectedComponent.Entity(() => OnSelectHorse(x.Key).Forget(), x.Value.HorseNtfId == current)
            }).ToArray()
        };
    }

    private async UniTaskVoid OnSelectHorse(long horseNtfId)
    {
        await QuickRaceDomainService.ChangeHorse(horseNtfId);
    }
}
