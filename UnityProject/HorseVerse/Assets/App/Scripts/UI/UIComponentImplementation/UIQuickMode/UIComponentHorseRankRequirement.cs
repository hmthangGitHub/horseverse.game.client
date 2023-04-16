using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseRankRequirement : UIComponentEnum<UIComponentHorseRankRequirement.Rarity>
{
    public enum Rarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legend
    }
}	