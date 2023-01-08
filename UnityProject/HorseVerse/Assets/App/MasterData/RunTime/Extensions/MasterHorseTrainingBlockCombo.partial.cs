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

    public Obstacle Clone()
    {
        return (Obstacle)this.MemberwiseClone();
    }
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
    
    public Coin Clone()
    {
        var target = new Position[benzierPointPositions.Length];
        benzierPointPositions.CopyTo(target, benzierPointPositions.Length);
        return new Coin()
        {
            localPosition = localPosition,
            numberOfCoin = numberOfCoin,
            benzierPointPositions = target
        };
    }
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
    public MasterHorseTrainingBlockCombo(long masterHorseTrainingBlockId, string name, MasterTrainingBlockComboType masterTrainingBlockComboType, string masterTrainingModularBlockId)
    {
        this.name = name;
        master_horse_training_block_id = masterHorseTrainingBlockId;
        master_horse_training_block_ids = masterTrainingModularBlockId;
        master_training_block_combo_type = masterTrainingBlockComboType;
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
        get
        {
            return JsonConvert.DeserializeObject(FormatCustomData(Obstacles), typeof(Obstacle[])) as Obstacle[] ?? Array.Empty<Obstacle>();
        }
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
        get
        {
            return JsonConvert.DeserializeObject(FormatCustomData(Coins), typeof(Coin[])) as Coin[] ?? Array.Empty<Coin>();
        }
#if ENABLE_MASTER_RUN_TIME_EDIT
        set
        {
            coins = JsonConvert.SerializeObject(value).Replace(",", "...");
        }
#endif
    }
    
#if ENABLE_MASTER_RUN_TIME_EDIT
    public void SetMasterTrainingModularBlockIdStart(string blockId)
    {
        master_training_modular_block_id_start = blockId;
    }
    
    public void SetMasterTrainingModularBlockIdEnd(string blockId)
    {
        master_training_modular_block_id_end = blockId;
    }
#endif

    private static string FormatCustomData(string s)
    {
        if (string.IsNullOrEmpty(s)) return "[]";
        return s.Replace("...", ",")
             .Replace("\"[", "[")
             .Replace("\"]", "]");
    }
}