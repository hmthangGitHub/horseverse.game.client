using io.hverse.game.protogen;

public class RaceMatchData
{
    public HorseRaceTime[] HorseRaceTimes { get; set; }
    public long MasterMapId { get; set; }
    public RaceMode Mode { get; set; }
    public long BetMatchId { get; set; }
    public int TotalBetWin { get; set; }
}

public class HorseRaceTime
{
    public float delayTime;
    public long masterHorseId;
    public RaceSegment[] raceSegments;
}

public class RaceSegment
{
    public int currentLane;
    public int toLane;
    public float time;
    public float percentage;
}    

public enum RaceMode
{
    BetMode,
    QuickMode
}
