using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpServerSelection : PopupEntity<UIPopUpServerSelection.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentInputField.Entity hostInput;
        public UIComponentInputField.Entity portInput;
        public ButtonComponent.Entity cancelBtn;
        public ButtonComponent.Entity connectBtn;
    }

    public UIComponentInputField hostInput;
    public UIComponentInputField portInput;
    public ButtonComponent cancelBtn;
    public ButtonComponent connectBtn;

    protected override void OnSetEntity()
    {
        hostInput.SetEntity(this.entity.hostInput);
        portInput.SetEntity(this.entity.portInput);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        connectBtn.SetEntity(this.entity.connectBtn);
    }
}
