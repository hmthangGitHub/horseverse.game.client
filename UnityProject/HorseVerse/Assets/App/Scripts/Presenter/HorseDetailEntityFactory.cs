using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseDetailEntityFactory
{
    private readonly IDIContainer container;
    private IReadOnlyHorseRepository horseRepository;
    private MasterHorseContainer masterHorseContainer;
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    public HorseDetailEntityFactory(IDIContainer container)
    {
        this.container = container;
    }

    public UIComponentHorseDetail.Entity InstantiateHorseDetailEntity(long horseNtfId)
    {
        var userHorse = HorseRepository.Models[horseNtfId];
        return new UIComponentHorseDetail.Entity()
        {
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
            horseRace = new UIComponentHorseRace.Entity()
            {
                type = GetHorseRace(UserDataRepository.Current.CurrentHorseNftId)
            }
        };
    }

    private int GetHorseRace(long horseNtfId)
    {
        var userHorse = HorseRepository.Models[horseNtfId];
        return (int)userHorse.HorseType;
    }
}
