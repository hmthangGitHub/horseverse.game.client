using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentHorseStableAvatar : UIComponent<UIComponentHorseStableAvatar.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public long horseNFTId;
        public string horseName;
        public UIComponentHorseRace.Entity horseRace;
        public ButtonSelectedComponent.Entity selectBtn;
    }

    public FormattedTextComponent horseName;
    public UIComponentHorseRace horseRace;
    public ButtonSelectedComponent selectBtn;

    protected override void OnSetEntity()
    {
        horseName.SetEntity(this.entity.horseName);
        selectBtn.SetEntity(this.entity.selectBtn);
        horseRace.SetEntity(this.entity.horseRace);
    }
}	