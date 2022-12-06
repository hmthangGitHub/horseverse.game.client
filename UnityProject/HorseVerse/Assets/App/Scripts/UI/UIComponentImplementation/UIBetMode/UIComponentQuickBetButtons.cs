using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentQuickBetButtons : UIComponent<UIComponentQuickBetButtons.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity betAllBtn;
    }

    public ButtonComponent betAllBtn;

    protected override void OnSetEntity()
    {
        betAllBtn.SetEntity(this.entity.betAllBtn);
    }
}	