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
    public RaceSegment[] raceSegments;
}

public class WayPoints
{
    public float time;
    public float percentage;
}

public class RaceSegment
{
    public int id;
    public int currentLane;
    public int toLane;
    public WayPoints[] waypoints;
}    

public enum RaceMode
{
    BetMode,
    QuickMode
}
