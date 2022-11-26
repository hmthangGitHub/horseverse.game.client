using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeUserInfo : UIComponent<UIComponentBetModeUserInfo.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public string userName;
        public long coin;
    }

    public FormattedTextComponent userName;
    public FormattedTextComponent coin;

    protected override void OnSetEntity()
    {
        userName.SetEntity(this.entity.userName);
        coin.SetEntity(this.entity.coin);
    }
}	