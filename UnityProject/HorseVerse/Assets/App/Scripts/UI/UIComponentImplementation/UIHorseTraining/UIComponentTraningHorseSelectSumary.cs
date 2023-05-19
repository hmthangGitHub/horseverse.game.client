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
        public UIComponentHorseRace.Entity horseRank;
        public UIComponentHorseRankBorder.Entity horseRankBorder;
        public ButtonSelectedComponent.Entity selectBtn;
        public UIImage.Entity horseAvatar;
    }

    public FormattedTextComponent horseName;
    public FormattedTextComponent horseLevel;
    public UIComponentHorseRace horseRace;
    public UIComponentHorseRace horseRank;
    public UIComponentHorseRankBorder horseRankBorder;
    public ButtonSelectedComponent selectBtn;
    public UIImage horseAvatar;

    protected override void OnSetEntity()
    {
        horseName.SetEntity(this.entity.horseName);
        horseLevel.SetEntity(this.entity.horseLevel);
        selectBtn.SetEntity(this.entity.selectBtn);
        horseRace.SetEntity(this.entity.horseRace);
        horseRank.SetEntity(this.entity.horseRank);
        horseRankBorder.SetEntity(this.entity.horseRankBorder);
        horseAvatar.SetEntity(this.entity.horseAvatar);
    }
}	