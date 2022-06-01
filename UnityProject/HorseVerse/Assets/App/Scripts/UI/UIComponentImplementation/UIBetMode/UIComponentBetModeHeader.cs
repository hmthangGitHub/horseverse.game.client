using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeHeader : UIComponent<UIComponentBetModeHeader.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public long timeCountDown;
        public UIComponentBetModeUserInfo.Entity userInfo;
        public FormattedTextComponent.Entity energy;
        public ButtonComponent.Entity changeRaceBtn;
    }

    public FormattedTextComponent timeCountDown;
    public UIComponentBetModeUserInfo userInfo;
    public FormattedTextComponent energy;
    public ButtonComponent changeRaceBtn;

    protected override void OnSetEntity()
    {
        timeCountDown.SetEntity(this.entity.timeCountDown);
        userInfo.SetEntity(this.entity.userInfo);
        energy.SetEntity(this.entity.energy);
        changeRaceBtn.SetEntity(this.entity.changeRaceBtn);
    }
}	