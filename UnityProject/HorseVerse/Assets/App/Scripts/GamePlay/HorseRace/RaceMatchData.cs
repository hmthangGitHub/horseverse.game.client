public class RaceMatchData
{
    public HorseRaceTime[] horseRaceTimes;
    public long masterMapId;
    public RaceMode mode;
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
