using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPopupSetName : PopupEntity<UIPopupSetName.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public string username;
        public ButtonComponent.Entity confirmBtn;
    }

    public TMP_InputField username;
    public ButtonComponent confirmBtn;

    protected override void OnSetEntity()
    {
        username.text = this.entity.username;
        confirmBtn.SetEntity(this.entity.confirmBtn);
    }

}
