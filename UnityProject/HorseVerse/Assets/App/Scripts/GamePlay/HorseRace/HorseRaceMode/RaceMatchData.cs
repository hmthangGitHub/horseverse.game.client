using io.hverse.game.protogen;

public class RaceMatchData
{
    public HorseRaceInfo[] HorseRaceInfos { get; set; }
}

public class HorseRaceInfo : HorseBriefInfo
{
    public float DelayTime { get; set; }
    public Rarity Rarity { get; set; }
    public HorseType Type { get; set; }
    public RaceSegmentTime[] RaceSegments { get; set; }
    
}

public class HorseBetInfo
{
    public HorseInfo[] HorseInfos { get; set; }
}

public class RaceSegmentTime
{
    public int CurrentLane { get; set; }
    public int ToLane { get; set; }
    public float Time { get; set; }
    public float Percentage { get; set; }
}

public interface IHorseBriefInfo
{
    long NftHorseId { get;} 
    MasterHorseMeshInformation MeshInformation { get;}
    string Name { get; }
    float Speed { get;}
    float Agility { get;}
    float Acceleration { get;}
    float SpeedRatio { get;}
    int Level { get;}
    float Stamina { get;}
    float TechnicallyRatio { get;}
}

public class HorseBriefInfo : IHorseBriefInfo
{
    public long NftHorseId { get; set; } 
    public MasterHorseMeshInformation MeshInformation { get; set; }
    public string Name { get; set; }
    public float Speed { get; set; }
    public float Agility { get; set; }
    public float Acceleration { get; set; }
    public float SpeedRatio { get; set; }
    public float Stamina { get; set; }
    public float TechnicallyRatio { get; set; }
    public int Level { get; set; }
}