using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseDetail : PopupEntity<UIHorseDetail.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public string horseName;
        public UIComponentProgressBarWithBonus.Entity speedProgressBarWithBonus;
        public UIComponentProgressBarWithBonus.Entity powerProgressBarWithBonus;
        public UIComponentProgressBarWithBonus.Entity technicallyProgressBarWithBonus;
        public int earning;
    }

    public FormattedTextComponent horseName;
    public UIComponentProgressBarWithBonus speedProgressBarWithBonus;
    public UIComponentProgressBarWithBonus powerProgressBarWithBonus;
    public UIComponentProgressBarWithBonus technicallyProgressBarWithBonus;
    public FormattedTextComponent earning;

    protected override void OnSetEntity()
    {
        horseName.SetEntity(this.entity.horseName);
        speedProgressBarWithBonus.SetEntity(this.entity.speedProgressBarWithBonus);
        powerProgressBarWithBonus.SetEntity(this.entity.powerProgressBarWithBonus);
        technicallyProgressBarWithBonus.SetEntity(this.entity.technicallyProgressBarWithBonus);
        earning.SetEntity(this.entity.earning);
    }
}	
