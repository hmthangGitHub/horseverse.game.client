using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpTypeRaceChoose : PopupEntity<UIPopUpTypeRaceChoose.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity outerBtn;
        public ButtonComponent.Entity closeBtn;
        public ButtonComponent.Entity raceTypeBtn;
        public ButtonComponent.Entity raceTypeBtn2;
        public ButtonComponent.Entity changeRaceTypeBtn;
    }

    public ButtonComponent outerBtn;
    public ButtonComponent closeBtn;
    public ButtonComponent raceTypeBtn;
    public ButtonComponent raceTypeBtn2;
    public ButtonComponent changeRaceTypeBtn;

    protected override void OnSetEntity()
    {
        outerBtn.SetEntity(this.entity.outerBtn);
        closeBtn.SetEntity(this.entity.closeBtn);
        raceTypeBtn.SetEntity(this.entity.raceTypeBtn);
        raceTypeBtn2.SetEntity(this.entity.raceTypeBtn2);
        changeRaceTypeBtn.SetEntity(this.entity.changeRaceTypeBtn);
    }
}	