using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrainingCoinCounting : PopupEntity<UITrainingCoinCounting.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int coin;
    }

    public FormattedTextComponent coin;

    protected override void OnSetEntity()
    {
	    coin.SetEntity(this.entity.coin);
    }
}	