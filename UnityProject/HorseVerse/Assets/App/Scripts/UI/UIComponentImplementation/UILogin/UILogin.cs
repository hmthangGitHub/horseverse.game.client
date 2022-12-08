using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogin : PopupEntity<UILogin.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentInputField.Entity id;
	    public UIComponentInputField.Entity passWord;
	    public ButtonComponent.Entity loginBtn;
        public ButtonComponent.Entity cancelBtn;
        public ButtonComponent.Entity registerBtn;
    }
    
    public UIComponentInputField id;
    public UIComponentInputField passWord;
    public ButtonComponent loginBtn;
    public ButtonComponent cancelBtn;
    public ButtonComponent registerBtn;

    protected override void OnSetEntity()
    { 
	    id.SetEntity(this.entity.id);
	    passWord.SetEntity(this.entity.passWord);
	    loginBtn.SetEntity(this.entity.loginBtn);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        registerBtn.SetEntity(this.entity.registerBtn);
    }
}	