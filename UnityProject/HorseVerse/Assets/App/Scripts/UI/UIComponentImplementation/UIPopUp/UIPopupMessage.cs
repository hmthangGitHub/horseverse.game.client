using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPopupMessage : PopupEntity<UIPopupMessage.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public string title;
        public string message;
        public ButtonComponent.Entity confirmBtn;
    }

    public TextMeshProUGUI title;
    public TextMeshProUGUI message;
    public ButtonComponent confirmBtn;

    protected override void OnSetEntity()
    {
        title.text = this.entity.title;
        message.text = this.entity.message;
        confirmBtn.SetEntity(this.entity.confirmBtn);
    }
}
