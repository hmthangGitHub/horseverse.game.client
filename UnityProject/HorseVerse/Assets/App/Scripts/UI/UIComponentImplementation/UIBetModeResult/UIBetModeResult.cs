using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetModeResult : PopupEntity<UIBetModeResult.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity nextBtn;
        public UIComponentBetModeResultList.Entity betModeResultList;
    }

    public ButtonComponent nextBtn;
    public UIComponentBetModeResultList betModeResultList;

    protected override void OnSetEntity()
    {
        nextBtn.SetEntity(this.entity.nextBtn);
        betModeResultList.SetEntity(this.entity.betModeResultList);
    }
}	