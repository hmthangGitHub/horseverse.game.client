using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpPause : PopupEntity<UIPopUpPause.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity settingBtn;
        public ButtonComponent.Entity continueBtn;
        public ButtonComponent.Entity exitBtn;
    }

    public ButtonComponent settingBtn;
    public ButtonComponent continueBtn;
    public ButtonComponent exitBtn;

    protected override void OnSetEntity()
    {
        settingBtn.SetEntity(this.entity.settingBtn);
        continueBtn.SetEntity(this.entity.continueBtn);
        exitBtn.SetEntity(this.entity.exitBtn);
    }

}
