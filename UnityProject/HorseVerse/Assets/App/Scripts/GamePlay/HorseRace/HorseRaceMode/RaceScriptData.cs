using io.hverse.game.protogen;

public class RaceScriptData
{
    public HorseRaceInfo[] HorseRaceInfos { get; set; }
    public long MasterMapId { get; set; }
}

public class HorseRaceInfo
{
    public string Name { get; set; }
    public float DelayTime { get; set; }
    public MasterHorseMeshInformation MeshInformation { get; set; }
    public RaceSegmentTime[] RaceSegments { get; set; }

    public float PowerBonus { get; set; }
    public float PowerRatio { get; set; }
    public float SpeedBonus { get; set; }
    public float SpeedRatio { get; set; }
    public float TechnicallyBonus { get; set; }
    public float TechnicallyRatio { get; set; }

    public int Type { get; set; }
    public int Rarity { get; set; }
    public int Level { get; set; }
}

public class HorseBetInfo
{
    public HorseDataModel[] horseInfos { get; set; }
}

public class RaceSegmentTime
{
    public int CurrentLane { get; set; }
    public int ToLane { get; set; }
    public float Time { get; set; }
    public float Percentage { get; set; }
}    
