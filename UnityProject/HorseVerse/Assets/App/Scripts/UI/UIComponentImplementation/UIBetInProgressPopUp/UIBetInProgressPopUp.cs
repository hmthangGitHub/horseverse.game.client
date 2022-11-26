using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetInProgressPopUp : PopupEntity<UIBetInProgressPopUp.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity outerBtn;
	    public UIComponentCountDownTimer.Entity timer;
    }
    
    public ButtonComponent outerBtn;
    public UIComponentCountDownTimer timer;

    protected override void OnSetEntity()
    {
	    outerBtn.SetEntity(this.entity.outerBtn);
	    timer.SetEntity(this.entity.timer);
    }
}	