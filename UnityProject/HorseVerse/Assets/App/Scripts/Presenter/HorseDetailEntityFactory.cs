using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseDetailEntityFactory
{
    private IDIContainer container;
    private IReadOnlyHorseRepository horseRepository;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    public HorseDetailEntityFactory(IDIContainer container)
    {
        this.container = container;
    }

    public UIComponentHorseDetail.Entity InstantiateHorseDetailEntity(long masterHorseId)
    {
        Debug.Log("Get Horse " + masterHorseId);
        var userHorse = HorseRepository.Get(masterHorseId); if (userHorse == null) return default;
        var masterHorse = MasterHorseContainer.MasterHorseIndexer[userHorse.MasterHorseResource];
        return new UIComponentHorseDetail.Entity()
        {
            earning = userHorse.Earning,
            horseName = masterHorse.Name,
            powerProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = userHorse.PowerBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = userHorse.PowerRatio
                }
            },
            speedProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = userHorse.SpeedBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = userHorse.SpeedRatio
                }
            },
            technicallyProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = userHorse.TechnicallyBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = userHorse.TechnicallyRatio
                }
            },
        };
    }
}
