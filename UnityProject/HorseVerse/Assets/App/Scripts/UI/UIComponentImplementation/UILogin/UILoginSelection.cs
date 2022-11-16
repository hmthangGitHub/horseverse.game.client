using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoginSelection : PopupEntity<UILoginSelection.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity emailLoginBtn;
        public ButtonComponent.Entity otpLoginBtn;
    }

    public ButtonComponent emailLoginBtn;
    public ButtonComponent otpLoginBtn;

    protected override void OnSetEntity()
    {
        emailLoginBtn.SetEntity(this.entity.emailLoginBtn);
        otpLoginBtn.SetEntity(this.entity.otpLoginBtn);
    }
}
