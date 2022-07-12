using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetReward : PopupEntity<UIBetReward.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public int reward;
        public ButtonComponent.Entity outerBtn;
    }

    public FormattedTextComponent reward;
    public ButtonComponent outerBtn;

    protected override void OnSetEntity()
    {
        reward.SetEntity(this.entity.reward);
        outerBtn.SetEntity(this.entity.outerBtn);
    }
}	