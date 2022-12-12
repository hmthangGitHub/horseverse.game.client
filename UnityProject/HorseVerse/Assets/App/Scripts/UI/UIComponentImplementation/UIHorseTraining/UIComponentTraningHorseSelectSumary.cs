using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentTraningHorseSelectSumary : UIComponent<UIComponentTraningHorseSelectSumary.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public long horseNFTId;
        public string horseName;
        public int horseLevel;
        public UIComponentHorseRace.Entity horseRace;
        public ButtonSelectedComponent.Entity selectBtn;
    }

    public FormattedTextComponent horseName;
    public FormattedTextComponent horseLevel;
    public UIComponentHorseRace horseRace;
    public ButtonSelectedComponent selectBtn;

    protected override void OnSetEntity()
    {
        horseName.SetEntity(this.entity.horseName);
        horseLevel.SetEntity(this.entity.horseLevel);
        selectBtn.SetEntity(this.entity.selectBtn);
        horseRace.SetEntity(this.entity.horseRace);
    }
}	