using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoginOTP : PopupEntity<UILoginOTP.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentInputField.Entity id;
        public UIComponentInputField.Entity code;
        public ButtonComponent.Entity loginBtn;
        public ButtonComponent.Entity cancelBtn;
        public ButtonComponent.Entity getCodeBtn;
    }

    public UIComponentInputField id;
    public UIComponentInputField code;
    public ButtonComponent loginBtn;
    public ButtonComponent cancelBtn;
    public ButtonComponent getCodeBtn;

    protected override void OnSetEntity()
    {
        id.SetEntity(this.entity.id);
        code.SetEntity(this.entity.code);
        loginBtn.SetEntity(this.entity.loginBtn);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        getCodeBtn.SetEntity(this.entity.getCodeBtn);
    }
}
