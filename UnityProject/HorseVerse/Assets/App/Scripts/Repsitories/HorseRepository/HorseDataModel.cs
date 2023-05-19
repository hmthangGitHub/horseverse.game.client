using io.hverse.game.protogen;
using UnityEngine;

public class HorseDataModel
{
    public long HorseNtfId => HorseBasic.Id;
    public string Name => HorseBasic.Name;
    public int Happiness => HorseRising.Happiness;

    public HorseType HorseType => (HorseType)HorseBasic.HorseType;
    public HorseRarity Rarity => (HorseRarity)HorseBasic.Rarity;
    public int HorseMasterId => 10000000 + HorseBasic.ColorType;
    public string HorseShortId => HorseBasic.ColorType.ToString("D4");

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

public static class HorseDataModelHelper
{
    public static string GetSpritePath(string HorseShortId)
    {
        return $"Avatar/horse_avatar/{HorseShortId}";
    }
}
