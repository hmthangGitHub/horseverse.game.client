using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpBetReward : PopupEntity<UIPopUpBetReward.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public int reward;
        public ButtonComponent.Entity confirmBtn;
    }

    public FormattedTextComponent reward;
    public ButtonComponent confirmBtn;

    protected override void OnSetEntity()
    {
        reward.SetEntity(this.entity.reward);
        confirmBtn.SetEntity(this.entity.confirmBtn);
    }
}	