
public enum HorseGameMode
{
    Bet,
    Race
}

public class HorseRaceContext
{
    public HorseGameMode GameMode { get; set; }
    public RaceScriptData RaceScriptData {get; set; }
    public RaceMatchDataContext RaceMatchDataContext { get; set; }
    public BetMatchDataContext BetMatchDataContext { get; set; }

    public void Reset()
    {
        GameMode = default;
        RaceScriptData = default;
        RaceMatchDataContext = default;
        BetMatchDataContext = default;
    }
}