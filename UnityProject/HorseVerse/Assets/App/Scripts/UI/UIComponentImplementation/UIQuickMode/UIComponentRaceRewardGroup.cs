using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentRaceRewardGroup : UIComponent<UIComponentRaceRewardGroup.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int chestNumber;
	    public int coinNumber;
    }

    public FormattedTextComponent chestNumber;
    public FormattedTextComponent coinNumber;
    
    protected override void OnSetEntity()
    {
	    chestNumber.SetEntity(this.entity.chestNumber);
	    coinNumber.SetEntity(this.entity.coinNumber);
    }
}	