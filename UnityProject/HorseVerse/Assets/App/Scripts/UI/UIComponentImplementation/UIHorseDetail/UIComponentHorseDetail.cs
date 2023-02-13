using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseDetail : UIComponent<UIComponentHorseDetail.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentHorseRace.Entity horseRace;
        public string horseName;
        public int level;
        public UIComponentProgressBarWithBonus.Entity speedProgressBarWithBonus;
        public UIComponentProgressBarWithBonus.Entity powerProgressBarWithBonus;
        public UIComponentProgressBarWithBonus.Entity technicallyProgressBarWithBonus;
        public int happiness;
        public int maxHappiness;
    }

    public UIComponentHorseRace horseRace;
    public FormattedTextComponent horseName;
    public UIComponentProgressBarWithBonus speedProgressBarWithBonus;
    public UIComponentProgressBarWithBonus powerProgressBarWithBonus;
    public UIComponentProgressBarWithBonus technicallyProgressBarWithBonus;
    public FormattedTextComponent happiness;
    public FormattedTextComponent level;

    protected override void OnSetEntity()
    {
        horseRace?.SetEntity(this.entity.horseRace);
        if(horseName != default)
            horseName.SetEntity(this.entity.horseName);
        speedProgressBarWithBonus.SetEntity(this.entity.speedProgressBarWithBonus);
        powerProgressBarWithBonus.SetEntity(this.entity.powerProgressBarWithBonus);
        technicallyProgressBarWithBonus.SetEntity(this.entity.technicallyProgressBarWithBonus);
        if(happiness != default) happiness.SetEntity(this.entity.happiness, this.entity.maxHappiness);
        level.SetEntity(this.entity.level);
    }
}	