using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataModel
{
    public int MaxHappinessNumber { get; set; }
    public int TrainingHappinessCost { get; set; }
    public List<int> BetNumberList { get; set; } = new List<int>();

    public MasterDataModel Clone()
    {
        return (MasterDataModel)this.MemberwiseClone();
    }
}
