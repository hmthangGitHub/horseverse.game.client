using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseInfoIntro : PopupEntity<UIHorseInfoIntro.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity outerBtn;
        public ButtonComponent.Entity skipAllBtn;
        public UIComponentHorseDetail.Entity horseInfo;
        public int gate;
    }

    public ButtonComponent outerBtn;
    public ButtonComponent skipAllBtn;
    public UIComponentHorseDetail horseInfo;
    public FormattedTextComponent gate;

    protected override void OnSetEntity()
    {
        outerBtn.SetEntity(this.entity.outerBtn);
        skipAllBtn.SetEntity(this.entity.skipAllBtn);
        horseInfo.SetEntity(this.entity.horseInfo);
        gate.SetEntity(this.entity.gate);
    }
}	