using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentMainMenuUserInfo : UIComponent<UIComponentMainMenuUserInfo.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public string userName;
        public int level;
        public string levelIcon;
        public int currentExp;
        public int maxExp;
        public UIComponentProgressBar.Entity levelProgressBar;
        public int energy;
        public int energyMax;
    }

    public FormattedTextComponent userName;
    public FormattedTextComponent level;
    public UIComponentImageLoader levelIcon;
    public FormattedTextComponent currentExp;
    public UIComponentProgressBar levelProgressBar;
    public FormattedTextComponent energy;

    protected override void OnSetEntity()
    {
        userName.SetEntity(this.entity.userName);
        level.SetEntity(this.entity.level);
        levelIcon.SetEntity(this.entity.levelIcon);
        currentExp.SetEntity(this.entity.currentExp, this.entity.maxExp);
        levelProgressBar.SetEntity(this.entity.levelProgressBar);
        energy.SetEntity(this.entity.energy, this.entity.energyMax);
    }
}	