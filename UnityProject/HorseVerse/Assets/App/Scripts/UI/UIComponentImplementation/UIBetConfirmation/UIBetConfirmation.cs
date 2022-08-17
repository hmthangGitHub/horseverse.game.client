using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetConfirmation : PopupEntity<UIBetConfirmation.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity cancelBtn;
	    public ButtonComponent.Entity confirmBtn;
	    public UIComponentToggle.Entity showAgain;
    }
    
    public ButtonComponent cancelBtn;
    public ButtonComponent confirmBtn;
    public UIComponentToggle showAgain;

    protected override void OnSetEntity()
    {
	    cancelBtn.SetEntity(this.entity.cancelBtn);
		confirmBtn.SetEntity(this.entity.confirmBtn);
	    showAgain.SetEntity(this.entity.showAgain);
    }
}	