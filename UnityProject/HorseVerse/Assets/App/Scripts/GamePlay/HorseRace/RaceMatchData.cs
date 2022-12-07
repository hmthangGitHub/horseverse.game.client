using io.hverse.game.protogen;

public class RaceMatchData
{
    public HorseRaceInfo[] HorseRaceInfos { get; set; }
    public long MasterMapId { get; set; }
    public RaceMode Mode { get; set; }
    public long BetMatchId { get; set; }
    public int TotalBetWin { get; set; }
}

public class HorseRaceInfo
{
    public string Name { get; set; }
    public float DelayTime { get; set; }
    public MasterHorseMeshInformation MeshInformation { get; set; }
    public RaceSegmentTime[] RaceSegments { get; set; }
}

public class HorseBetInfo
{
    public HorseDataModel[] horseInfos { get; set; }
}

public class RaceSegmentTime
{
    public int currentLane;
    public int ToLane { get; set; }
    public float Time { get; set; }
    public float Percentage { get; set; }
}    

public enum RaceMode
{
    Bet,
    Race
}
