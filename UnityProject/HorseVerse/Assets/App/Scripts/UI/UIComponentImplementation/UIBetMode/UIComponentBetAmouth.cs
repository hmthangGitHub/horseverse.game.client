using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetAmouth : UIComponent<UIComponentBetAmouth.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public int betAmouth;
        public ButtonSelectedComponent.Entity button;
    }

    public List<FormattedTextComponent> betAmouth;
    public ButtonSelectedComponent button;

    protected override void OnSetEntity()
    {
        button.SetEntity(this.entity.button);
        foreach (var item in betAmouth)
        {
            item.SetEntity(this.entity.betAmouth);
        }
    }
}	