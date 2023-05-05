using System.Collections;
using System.Collections.Generic;
using System.Linq;
using io.hverse.game.protogen;
using UnityEngine;

public class MasterDataModel
{
    public int MaxHappinessNumber { get; set; }
    public int TrainingHappinessCost { get; set; }
    public List<int> BetNumberList { get; set; } = new List<int>();
    public Dictionary<(RacingRoomType roomType, int rank), RewardInfo[]> RacingRewardInfos { get; set; }
    public int MaxDailyRacingNumber { get; set; }
    public int MaxBreedingNumber  { get; set; }
    public int MaxCoinCollected  { get; set; }
    public BreedingFee[] BreedingFees  { get; set; }
    public float BreedingAttributeFactor  { get; set; }
    
    public MasterDataModel Clone()
    {
        return (MasterDataModel)this.MemberwiseClone();
    }
}

public enum RacingRoomType
{
    None,
    Novice,
    Basic,
    Advance,
    Expert,
    Master
}
