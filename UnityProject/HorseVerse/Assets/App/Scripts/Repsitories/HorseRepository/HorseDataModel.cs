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

    public int Type { get; set; }
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
