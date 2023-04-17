﻿using System.Linq;
using UnityEngine;

public class HorseRaceThirdPersonDataFactory
{
    private readonly IDIContainer container;
    private MasterHorseContainer masterHorseContainer;
    private HorseRepository horseRepository;
    private UserDataRepository userDataRepository;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private HorseRepository HorseRepository => horseRepository ??= container.Inject<HorseRepository>();
    private UserDataRepository UserDataRepository => userDataRepository ??= container.Inject<UserDataRepository>();

    public HorseRaceThirdPersonDataFactory(IDIContainer container)
    {
        this.container = container;
    }

    public HorseRaceThirdPersonInfo[] CreateHorseRaceInfo()
    {
        return Enumerable.Range(0, 7)
                         .Select(_ => CreateRandomHorseInfo())
                         .Append(CreateUserHorseData())
                         .Shuffle()
                         .ToArray();
    }

    private HorseRaceThirdPersonInfo CreateUserHorseData()
    {
        var userHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
        var masterHorse = MasterHorseContainer.FromTypeToMasterHorse(userHorse.HorseType);
        return new HorseRaceThirdPersonInfo()
        {
            Name = userHorse.Name,
            MeshInformation = new MasterHorseMeshInformation()
            {
                color1 = userHorse.Color1,
                color2 = userHorse.Color2,
                color3 = userHorse.Color3,
                color4 = userHorse.Color4,
                masterHorseId = masterHorse.MasterHorseId
            },
            PowerBonus = userHorse.PowerBonus,
            PowerRatio = userHorse.PowerRatio,
            SpeedBonus = userHorse.SpeedBonus,
            SpeedRatio = userHorse.SpeedRatio,
            TechnicallyBonus = userHorse.TechnicallyBonus,
            TechnicallyRatio = userHorse.TechnicallyRatio,
            NftHorseId = userHorse.HorseNtfId,
            HorseRaceThirdPersonStats = CreateRandomHorseRaceThirdPersonStats()
        };
    }

    private HorseRaceThirdPersonInfo CreateRandomHorseInfo()
    {
        return new HorseRaceThirdPersonInfo()
        {
            Level = Random.Range(1, 5),
            Name = "Random Horse Name",
            MeshInformation = new MasterHorseMeshInformation()
            {
                color1 = RandomColor(),
                color2 = RandomColor(),
                color3 = RandomColor(),
                color4 = RandomColor(),
                masterHorseId = MasterHorseContainer.MasterHorseIndexer.Keys.RandomElement()
            },
            PowerBonus = Random.value,
            PowerRatio = Random.value,
            SpeedBonus = Random.value,
            SpeedRatio = Random.value,
            TechnicallyBonus = Random.value,
            TechnicallyRatio = Random.value,
            NftHorseId = (long)Random.Range((float)0, Mathf.Infinity),
            HorseRaceThirdPersonStats = CreateRandomHorseRaceThirdPersonStats()
        };
    }

    private HorseRaceThirdPersonStats CreateRandomHorseRaceThirdPersonStats()
    {
        return new HorseRaceThirdPersonStats()
        {
            AccelerationRange = new Vector2(Random.Range(1f, 2f), Random.Range(3f, 4f)),
            HorizontalSpeed = Mathf.Lerp(3, 3, Random.value),
            SprintChargeNumber = Random.Range(1, 5),
            SprintTime = 4f,
            ForwardSpeedRange = new Vector2(Random.Range(10f, 11f), Random.Range(20f, 21f)),
            SprintHealingTime = 8f,
            PercentageSpeedBoostWhenSprint = 0.05f,
            PercentageSpeedBonusBoostWhenSprintContinuously = 0.005f,
            SprintBonusTime = 2.0f
        };
    }

    private Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}