using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentQuickBetButtons : UIComponent<UIComponentQuickBetButtons.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity betAllBtn;
        public ButtonComponent.Entity verticalBetBtn;
        public ButtonComponent.Entity horizontalBetBtn;
    }

    public ButtonComponent betAllBtn;
    public ButtonComponent verticalBetBtn;
    public ButtonComponent horizontalBetBtn;

    protected override void OnSetEntity()
    {
        betAllBtn.SetEntity(this.entity.betAllBtn);
        verticalBetBtn.SetEntity(this.entity.verticalBetBtn);
        horizontalBetBtn.SetEntity(this.entity.horizontalBetBtn);
    }
}	