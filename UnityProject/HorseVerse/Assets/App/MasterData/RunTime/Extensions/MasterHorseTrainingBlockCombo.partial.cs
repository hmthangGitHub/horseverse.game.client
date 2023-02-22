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

[Serializable]
public class Trap
{
    [JsonProperty]
    public string type;
    [JsonProperty]
    public string id;
    [JsonProperty]
    public Position localPosition;
    [JsonProperty]
    public string extraData;

    public Trap Clone()
    {
        return (Trap)this.MemberwiseClone();
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
#if ENABLE_DEBUG_MODULE
    public MasterHorseTrainingBlockCombo(long masterHorseTrainingBlockId, 
                                         string name, 
                                         MasterTrainingBlockComboType masterTrainingBlockComboType, 
                                         string masterTrainingModularBlockId)
    {
        this.name = name;
        master_horse_training_block_id = masterHorseTrainingBlockId;
        master_horse_training_block_ids = masterTrainingModularBlockId;
        master_training_block_combo_type = masterTrainingBlockComboType;
    }

    public MasterHorseTrainingBlockCombo Clone(long overrideMasterHorseTrainingBlockId)
    {
        var clone = (MasterHorseTrainingBlockCombo)this.MemberwiseClone();
        clone.master_horse_training_block_id = overrideMasterHorseTrainingBlockId;
        return clone;
    }
#endif
    
    public string[] MasterHorseTrainingBlockIdList
    {
        get => CreateMasterHorseTrainingBlockIdList();
#if ENABLE_DEBUG_MODULE
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
#if ENABLE_DEBUG_MODULE
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
#if ENABLE_DEBUG_MODULE
        set
        {
            coins = JsonConvert.SerializeObject(value).Replace(",", "...");
        }
#endif
    }

    public Trap[] TrapList
    {
        get
        {
            return JsonConvert.DeserializeObject(FormatCustomData(Traps), typeof(Trap[])) as Trap[] ?? Array.Empty<Trap>();
        }
#if ENABLE_DEBUG_MODULE
        set
        {
            traps = JsonConvert.SerializeObject(value).Replace(",", "...");
        }
#endif
    }

#if ENABLE_DEBUG_MODULE
    public void SetMasterTrainingModularBlockIdStart(string blockId)
    {
        master_training_modular_block_id_start = blockId;
    }
    
    public void SetMasterTrainingModularBlockIdEnd(string blockId)
    {
        master_training_modular_block_id_end = blockId;
    }
#endif

    public static string FormatCustomData(string s)
    {
        if (string.IsNullOrEmpty(s)) return "[]";
        return s.Replace("...", ",")
             .Replace("\"[", "[")
             .Replace("\"]", "]");
    }
}