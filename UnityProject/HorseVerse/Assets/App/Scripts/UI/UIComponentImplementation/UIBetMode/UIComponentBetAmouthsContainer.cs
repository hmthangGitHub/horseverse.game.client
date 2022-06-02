using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetAmouthsContainer : UIComponent<UIComponentBetAmouthsContainer.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public int totalBetAmouth;
        public ButtonComponent.Entity cancelBtn;
        public UIComponentBetAmouthIndicator.Entity betAmouthIndicator;
    }

    public FormattedTextComponent totalBetAmouth;
    public ButtonComponent cancelBtn;
    public UIComponentBetAmouthIndicator betAmouthIndicator;

    protected override void OnSetEntity()
    {
        totalBetAmouth.SetEntity(this.entity.totalBetAmouth);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        betAmouthIndicator.SetEntity(this.entity.betAmouthIndicator);
    }
}	