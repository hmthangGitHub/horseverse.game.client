using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChooseRaceMode : PopupEntity<UIChooseRaceMode.Entity>
{
    [Serializable]
    public class Entity
    {
        public ButtonComponent.Entity backBtn;
        public ButtonComponent.Entity quickRaceBtn;
        public ButtonComponent.Entity traningBtn;
    }

    public ButtonComponent backBtn;
    public ButtonComponent quickRaceBtn;
    public ButtonComponent traningBtn;

    protected override void OnSetEntity()
    {
        backBtn.SetEntity(this.entity.backBtn);
        quickRaceBtn.SetEntity(this.entity.quickRaceBtn);
        traningBtn.SetEntity(this.entity.traningBtn);
    }
}
