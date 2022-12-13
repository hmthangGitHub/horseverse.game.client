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
    }

    public UIComponentInputField hostInput;
    public UIComponentInputField portInput;
    public ButtonComponent cancelBtn;
    public ButtonComponent connectBtn;
    public ButtonComponent localBtn;
    public ButtonComponent mainBtn;

    private string defaultHost;
    private string defaultPort;
    private string localHost = "localhost";
    private string localPort = "8770";
    protected override void OnSetEntity()
    {
        hostInput.SetEntity(this.entity.hostInput);
        portInput.SetEntity(this.entity.portInput);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        connectBtn.SetEntity(this.entity.connectBtn);
        defaultHost = this.entity.hostInput.defaultValue;
        defaultPort = this.entity.portInput.defaultValue;
        localBtn.SetEntity(new ButtonComponent.Entity(()=> OnBtnLocalClicked()));
        mainBtn.SetEntity(new ButtonComponent.Entity(() => OnBtnMainClicked()));
    }

    public void OnToggleLoginType(int index)
    {
        this.entity.CurrentProfileIndex = index;
    }

    private void OnBtnLocalClicked()
    {
        hostInput.inputField.text = localHost;
        portInput.inputField.text = localPort;
    }

    private void OnBtnMainClicked()
    {
        hostInput.inputField.text = defaultHost;
        portInput.inputField.text = defaultPort;
    }
}
