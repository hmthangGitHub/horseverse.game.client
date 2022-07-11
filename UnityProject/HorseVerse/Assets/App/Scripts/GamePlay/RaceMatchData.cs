public class RaceMatchData
{
    public HorseRaceTime[] horseRaceTimes;
    public long masterMapId;
    public RaceMode mode;
}

public class HorseRaceTime
{
    public long masterHorseId;
    public float time;
}

public enum RaceMode
{
    BetMode,
    QuickMode
}
