﻿using io.hverse.game.protogen;
using UnityEngine;

public class HorseDataModel
{
    public long HorseNtfId { get; set; }
    public string Name { get; set; }
    public int Happiness { get; set; }
    public int Earning { get; set; }
    public float PowerBonus { get; set; }
    public float PowerRatio { get; set; }
    public float SpeedBonus { get; set; }
    public float SpeedRatio { get; set; }
    public float TechnicallyBonus { get; set; }
    public float TechnicallyRatio { get; set; }

    public HorseType Type { get; set; }
    public HorseRarity Rarity { get; set; }
    public int Level { get; set; }

    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public Color Color3 { get; set; }
    public Color Color4 { get; set; }

    public float LastBettingRecord { get; set; }
    public float AverageBettingRecord { get; set; }
    public float BestBettingRecord { get; set; }
    public float Rate { get; set; }
    public float HorizontalSpeed { get; set; }
    public Vector2 ForwardSpeedRange { get; set; }
    public float PercentageSpeedBoostWhenSprint { get; set; }
    public float PercentageSpeedBonusBoostWhenSprintContinuously { get; set; }
    public Vector2 AccelerationRange { get; set; }
    public float SprintTime { get; set; }
    public int SprintChargeNumber { get; set; }
    public float SprintHealingTime { get; set; }
    public float SprintBonusTime { get; set; }
    public HorseBasic HorseBasic { get; set; }
    public HorseAttribute HorseAttribute { get; set; }
    public HorseRising HorseRising { get; set; }
    public HorseHistory HorseHistory { get; set; }

    public HorseDataModel Clone()
    {
        return (HorseDataModel)this.MemberwiseClone();
    }
}

public enum HorseRarity
{
    None,
    Common,
    Uncommon,
    Rare,
    Epic,
    Legend
}
