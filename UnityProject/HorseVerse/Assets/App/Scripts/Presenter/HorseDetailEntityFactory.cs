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

    public UIComponentHorseDetail.Entity InstantiateHorseDetailEntity(long horseNtfId)
    {
        var userHorse = HorseRepository.Models[horseNtfId];
        var masterHorse = MasterHorseContainer.FromTypeToMasterHorse(userHorse.Type);
        return new UIComponentHorseDetail.Entity()
        {
            level = userHorse.Level,
            horseName = userHorse.Name,
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
            happiness = userHorse.Happiness,
            maxHappiness = UserSettingLocalRepository.MasterDataModel.MaxHappinessNumber,
        };
    }

    public int GetHorseRace(long horseNtfId)
    {
        var userHorse = HorseRepository.Models[horseNtfId];
        return userHorse.Type;
    }
}
