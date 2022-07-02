using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseInfoIntro : PopupEntity<UIHorseInfoIntro.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity outerBtn;
        public ButtonComponent.Entity skipAllBtn;
        public string horseName;
        public int gate;
    }

    public ButtonComponent outerBtn;
    public ButtonComponent skipAllBtn;
    public FormattedTextComponent horseName;
    public FormattedTextComponent gate;

    protected override void OnSetEntity()
    {
        outerBtn.SetEntity(this.entity.outerBtn);
        skipAllBtn.SetEntity(this.entity.skipAllBtn);
        horseName.SetEntity(this.entity.horseName);
        gate.SetEntity(this.entity.gate);
    }
}	