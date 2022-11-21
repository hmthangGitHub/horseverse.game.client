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
        return new UIComponentTraningHorseSelectSumaryList.Entity()
        {
            entities = HorseRepository.Models.Select(x => new UIComponentTraningHorseSelectSumary.Entity()
            {
                horseName = MasterHorseContainer.MasterHorseIndexer[x.Value.MasterHorseId].Name,
                selectBtn = new ButtonComponent.Entity(() => OnSelectHorse(x.Key))
            }).ToArray()
        };
    }

    private void OnSelectHorse(long horseNtfId)
    {
        QuickRaceDomainService.ChangeHorse(horseNtfId).Forget();
    }
}
