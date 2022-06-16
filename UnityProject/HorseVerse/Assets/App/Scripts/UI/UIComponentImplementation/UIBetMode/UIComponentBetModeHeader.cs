using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeHeader : UIComponent<UIComponentBetModeHeader.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentCountDownTimer.Entity timeCountDown;
        public UIComponentBetModeUserInfo.Entity userInfo;
        public int energy;
        public int maxEnergy;
        public ButtonComponent.Entity changeRaceBtn;
    }

    public UIComponentCountDownTimer timeCountDown;
    public UIComponentBetModeUserInfo userInfo;
    public FormattedTextComponent energy;
    public ButtonComponent changeRaceBtn;

    protected override void OnSetEntity()
    {
        timeCountDown.SetEntity(this.entity.timeCountDown);
        userInfo.SetEntity(this.entity.userInfo);
        energy.SetEntity(this.entity.energy, this.entity.maxEnergy);
        changeRaceBtn.SetEntity(this.entity.changeRaceBtn);
    }
}	