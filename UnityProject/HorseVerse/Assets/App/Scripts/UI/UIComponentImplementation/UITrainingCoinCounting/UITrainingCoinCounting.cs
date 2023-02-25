using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrainingCoinCounting : PopupEntity<UITrainingCoinCounting.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int coin;
        public int point;
        public ButtonComponent.Entity btnSetting;
    }

    public FormattedTextComponent coin;
    public FormattedTextComponent point;
    public ButtonComponent btnSetting;

    protected override void OnSetEntity()
    {
	    coin.SetEntity(this.entity.coin);
        point.SetEntity(this.entity.point);
        btnSetting.SetEntity(this.entity.btnSetting);
    }
}	