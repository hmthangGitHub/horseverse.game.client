using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuItem : UIComponent<UIDebugMenuItem.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity debugMenuBtn;
	    public string debugMenu;
    }
    
    public ButtonComponent debugMenuBtn;
    public FormattedTextComponent debugMenu;

    protected override void OnSetEntity()
    {
	    debugMenuBtn.SetEntity(this.entity.debugMenuBtn);
	    debugMenu.SetEntity(this.entity.debugMenu);
    }
}	