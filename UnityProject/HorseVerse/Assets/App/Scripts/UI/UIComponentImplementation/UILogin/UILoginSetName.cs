using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILoginSetName : PopupEntity<UILoginSetName.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public string username;
        public System.Action<string> onUpdateInput;
        public ButtonComponent.Entity confirmBtn;
    }

    public TMP_InputField username;
    public ButtonComponent confirmBtn;

    protected override void OnSetEntity()
    {
        username.onValueChanged.RemoveAllListeners();
        username.text = this.entity.username;
        username.onValueChanged.AddListener(value => this.entity.onUpdateInput(value));
        confirmBtn.SetEntity(this.entity.confirmBtn);
    }

    private void OnDestroy()
    {
        if(username!= default)
            username.onValueChanged.RemoveAllListeners();
    }


}
