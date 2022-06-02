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
    }

    public UIComponentHorseDetail horseDetail;
    public ButtonComponent levelUpBtn;

    protected override void OnSetEntity()
    {
        horseDetail.SetEntity(this.entity.horseDetail);
        levelUpBtn.SetEntity(this.entity.levelUpBtn);
    }
}	
