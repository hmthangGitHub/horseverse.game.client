using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeHeader : UIComponent<UIComponentBetModeHeader.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentCountDownTimer.Entity timeCountDown;
        public ButtonComponent.Entity changeRaceBtn;
    }

    public UIComponentCountDownTimer timeCountDown;
    public ButtonComponent changeRaceBtn;

    protected override void OnSetEntity()
    {
        timeCountDown.SetEntity(this.entity.timeCountDown);
        changeRaceBtn.SetEntity(this.entity.changeRaceBtn);
    }
}	