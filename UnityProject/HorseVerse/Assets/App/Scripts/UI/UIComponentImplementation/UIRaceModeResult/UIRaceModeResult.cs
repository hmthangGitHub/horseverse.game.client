using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRaceModeResult : PopupEntity<UIRaceModeResult.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity nextBtn;
        public UIComponentBetModeResultList.Entity betModeResultList;
        public bool rewardTitle;
    }

    public ButtonComponent nextBtn;
    public UIComponentBetModeResultList betModeResultList;
    public IsVisibleComponent rewardTitle;

    protected override void OnSetEntity()
    {
        nextBtn.SetEntity(this.entity.nextBtn);
        betModeResultList.SetEntity(this.entity.betModeResultList);
        rewardTitle.SetEntity(this.entity.rewardTitle);
    }
}