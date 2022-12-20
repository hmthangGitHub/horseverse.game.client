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
        public int CurrentProfileIndex;
        public ServerDefine serverDefine;
    }

    public UIComponentInputField hostInput;
    public UIComponentInputField portInput;
    public ButtonComponent cancelBtn;
    public ButtonComponent connectBtn;
    public ButtonComponent localBtn;
    public ButtonComponent devBtn;
    public ButtonComponent stagingBtn;

    protected override void OnSetEntity()
    {
        hostInput.SetEntity(this.entity.hostInput);
        portInput.SetEntity(this.entity.portInput);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        connectBtn.SetEntity(this.entity.connectBtn);
        localBtn.SetEntity(new ButtonComponent.Entity(()=> OnBtnLocalClicked()));
        devBtn.SetEntity(new ButtonComponent.Entity(() => OnBtnDevClicked()));
        stagingBtn.SetEntity(new ButtonComponent.Entity(() => OnBtnMainClicked()));
    }

    public void OnToggleLoginType(int index)
    {
        this.entity.CurrentProfileIndex = index;
    }

    private void OnBtnLocalClicked()
    {
        hostInput.inputField.text = this.entity.serverDefine.Local.Host;
        portInput.inputField.text = this.entity.serverDefine.Local.Port.ToString();
    }

    private void OnBtnDevClicked()
    {
        hostInput.inputField.text = this.entity.serverDefine.Dev.Host;
        portInput.inputField.text = this.entity.serverDefine.Dev.Port.ToString();
    }

    private void OnBtnMainClicked()
    {
        hostInput.inputField.text = this.entity.serverDefine.Staging.Host;
        portInput.inputField.text = this.entity.serverDefine.Staging.Port.ToString();
    }
}
