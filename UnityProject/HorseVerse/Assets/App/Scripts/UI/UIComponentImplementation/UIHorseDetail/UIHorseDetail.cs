using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseDetail : PopupEntity<UIHorseDetail.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentHorseDetail.Entity horseDetail;
        public ButtonComponent.Entity levelUpBtn;
        public ButtonComponent.Entity backBtn;
    }

    public UIComponentHorseDetail horseDetail;
    public ButtonComponent levelUpBtn;
    public ButtonComponent backBtn;

    protected override void OnSetEntity()
    {
        horseDetail.SetEntity(this.entity.horseDetail);
        levelUpBtn.SetEntity(this.entity.levelUpBtn);
        backBtn.SetEntity(this.entity.backBtn);
    }
}	
