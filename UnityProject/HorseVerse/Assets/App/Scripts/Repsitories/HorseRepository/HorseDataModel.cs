﻿using io.hverse.game.protogen;
using UnityEngine;

public class HorseDataModel
{
    public long HorseNtfId => HorseBasic.Id;
    public string Name => HorseBasic.Name;
    public int Happiness => HorseRising.Happiness;
    public int Earning { get; set; }
    public float PowerBonus { get; set; }
    public float PowerRatio { get; set; }
    public float SpeedBonus { get; set; }
    public float SpeedRatio { get; set; }
    public float TechnicallyBonus { get; set; }
    public float TechnicallyRatio { get; set; }

    public HorseType HorseType => (HorseType)HorseBasic.HorseType;
    public HorseRarity Rarity => (HorseRarity)HorseBasic.Rarity;

    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public Color Color3 { get; set; }
    public Color Color4 { get; set; }

    public float LastBettingRecord { get; set; }
    public float AverageBettingRecord { get; set; }
    public float BestBettingRecord { get; set; }
    public float Rate { get; set; }
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
