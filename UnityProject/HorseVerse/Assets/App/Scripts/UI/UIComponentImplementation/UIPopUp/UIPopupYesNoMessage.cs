using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPopupYesNoMessage : PopupEntity<UIPopupYesNoMessage.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public string title;
        public string message;
        public ButtonComponent.Entity yesBtn;
        public ButtonComponent.Entity noBtn;
    }

    public TextMeshProUGUI title;
    public TextMeshProUGUI message;
    public ButtonComponent yesBtn;
    public ButtonComponent noBtn;

    protected override void OnSetEntity()
    {
        title.text = this.entity.title;
        message.text = this.entity.message;
        yesBtn.SetEntity(this.entity.yesBtn);
        noBtn.SetEntity(this.entity.noBtn);
    }

}
