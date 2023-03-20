
public class HorseRaceThirdPersonMatchData
{
    public HorseRaceThirdPersonInfo[] HorseRaceInfos { get; set; }
    public long MasterMapId { get; set; }
}

public class HorseRaceThirdPersonInfo : HorseBriefInfo
{
    public HorseRaceThirdPersonStats HorseRaceThirdPersonStats { get; set; }
}