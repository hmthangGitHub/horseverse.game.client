using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeHeader : UIComponent<UIComponentBetModeHeader.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentCountDownTimer.Entity timeCountDown;
        public UIHeader.Entity header;
        public ButtonComponent.Entity changeRaceBtn;
    }

    public UIComponentCountDownTimer timeCountDown;
    public UIHeader header;
    public ButtonComponent changeRaceBtn;

    protected override void OnSetEntity()
    {
        timeCountDown.SetEntity(this.entity.timeCountDown);
        header.SetEntity(this.entity.header);
        changeRaceBtn.SetEntity(this.entity.changeRaceBtn);
    }
}	