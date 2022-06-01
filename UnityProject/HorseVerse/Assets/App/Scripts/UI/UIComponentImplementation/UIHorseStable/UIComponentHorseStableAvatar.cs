using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseStableAvatar : UIComponent<UIComponentHorseStableAvatar.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity selectBtn;
    }

    public ButtonComponent selectBtn;

    protected override void OnSetEntity()
    {
        this.selectBtn.SetEntity(this.entity.selectBtn);
    }
}	