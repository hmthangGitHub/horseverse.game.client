using UnityEngine;

public class HorseDataModel
{
    public long MasterHorseId { get; set; }
    public int Earning { get; set; }
    public float PowerBonus { get; set; }
    public float PowerRatio { get; set; }
    public float SpeedBonus { get; set; }
    public float SpeedRatio { get; set; }
    public float TechnicallyBonus { get; set; }
    public float TechnicallyRatio { get; set; }

    public int Type { get; set; }
    public int Rarity { get; set; }
    public int Level { get; set; }

    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public Color Color3 { get; set; }
    public Color Color4 { get; set; }
    public long MasterHorseResource { get { return 10000000 + Rarity * 10 + Type; } }
}