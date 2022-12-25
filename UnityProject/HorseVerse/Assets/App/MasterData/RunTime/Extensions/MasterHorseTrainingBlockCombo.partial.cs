using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class Obstacle
{
    [JsonProperty]
    public string type;
    [JsonProperty] 
    public Position localPosition;
}

[Serializable]
public class Coin
{
    [JsonProperty]
    public int numberOfCoin;
    [JsonProperty] 
    public Position localPosition;
    [JsonProperty]
    public Position[] benzierPointPositions;
}

public class Position
{
    [JsonProperty] public float x;
    [JsonProperty] public float y;
    [JsonProperty] public float z;

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    
    public static Position FromVector3(Vector3 vector)
    {
        return new Position()
        {
            x = vector.x,
            y = vector.y,
            z = vector.z,
        };
    }
}

public partial class MasterHorseTrainingBlockCombo
{
    public MasterHorseTrainingBlockCombo(long masterHorseTrainingBlockId, string name)
    {
        this.name = name;
        this.master_horse_training_block_id = masterHorseTrainingBlockId;
        master_horse_training_block_ids = string.Empty;
    }
    
    public string[] MasterHorseTrainingBlockIdList
    {
        get => CreateMasterHorseTrainingBlockIdList();
#if ENABLE_MASTER_RUN_TIME_EDIT
        set => master_horse_training_block_ids = string.Join(".", value);
#endif
    }
    
    private string[] CreateMasterHorseTrainingBlockIdList()
    {
        return !string.IsNullOrEmpty(MasterHorseTrainingBlockIds)
            ? MasterHorseTrainingBlockIds.Split('.')
                .ToArray() 
            : Array.Empty<string>();
    }

    public Obstacle[] ObstacleList
    {
        get =>
            JsonConvert.DeserializeObject((Obstacles ?? string.Empty).Replace("...", ","), typeof(Obstacle[])) as
                Obstacle[] ?? Array.Empty<Obstacle>();  
#if ENABLE_MASTER_RUN_TIME_EDIT
        set
        {
            obstacles = JsonConvert.SerializeObject(value)
                                   .Replace(",", "...");
        }
#endif
    }

    public Coin[] CoinList
        {
            get => JsonConvert.DeserializeObject((Coins ?? string.Empty).Replace("...", ","), typeof(Coin[])) as Coin[] ?? Array.Empty<Coin>();
#if ENABLE_MASTER_RUN_TIME_EDIT
            set
            {
                coins = JsonConvert.SerializeObject(value).Replace(",", "...");
            }
#endif
        }
}