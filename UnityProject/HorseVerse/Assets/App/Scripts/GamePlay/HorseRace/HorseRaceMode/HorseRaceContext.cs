
public enum HorseGameMode
{
    Bet,
    Race
}

public class HorseRaceContext
{
    public HorseGameMode GameMode { get; set; }
    public long MasterMapId { get; set; }
    public IHorseBriefInfo[] HorseBriefInfos { get; set; }
    public RaceMatchData RaceMatchData {get; set; }
    public HorseRaceThirdPersonMatchData HorseRaceThirdPersonMatchData {get; set; }
    public RaceMatchDataContext RaceMatchDataContext { get; set; }
    public BetMatchDataContext BetMatchDataContext { get; set; }

    public void Reset()
    {
        GameMode = default;
        RaceMatchData = default;
        HorseBriefInfos = default;
        RaceMatchDataContext = default;
        BetMatchDataContext = default;
        HorseRaceThirdPersonMatchData = default;
        MasterMapId = default;
    }
}