using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStable : PopupEntity<UIHorseStable.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentHorseStableAvatarList.Entity stableHorseAvatarList;
        public ButtonComponent.Entity backBtn;
    }

    public UIComponentHorseStableAvatarList stableHorseAvatarList;
    public ButtonComponent backBtn;

    protected override void OnSetEntity()
    {
        stableHorseAvatarList.SetEntity(this.entity.stableHorseAvatarList);
        backBtn.SetEntity(this.entity.backBtn);
    }
}	