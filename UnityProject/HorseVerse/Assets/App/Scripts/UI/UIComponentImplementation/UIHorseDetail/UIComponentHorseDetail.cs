using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseDetail : UIComponent<UIComponentHorseDetail.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public string horseName;
        public int level;
        public UIComponentProgressBarWithBonus.Entity speedProgressBarWithBonus;
        public UIComponentProgressBarWithBonus.Entity powerProgressBarWithBonus;
        public UIComponentProgressBarWithBonus.Entity technicallyProgressBarWithBonus;
        //public int earning;
    }

    public FormattedTextComponent horseName;
    public UIComponentProgressBarWithBonus speedProgressBarWithBonus;
    public UIComponentProgressBarWithBonus powerProgressBarWithBonus;
    public UIComponentProgressBarWithBonus technicallyProgressBarWithBonus;
    //public FormattedTextComponent earning;
    public FormattedTextComponent level;

    protected override void OnSetEntity()
    {
        if(horseName != default)
            horseName.SetEntity(this.entity.horseName);
        speedProgressBarWithBonus.SetEntity(this.entity.speedProgressBarWithBonus);
        powerProgressBarWithBonus.SetEntity(this.entity.powerProgressBarWithBonus);
        technicallyProgressBarWithBonus.SetEntity(this.entity.technicallyProgressBarWithBonus);
        //earning.SetEntity(this.entity.earning);
        level.SetEntity(this.entity.level);
    }
}	