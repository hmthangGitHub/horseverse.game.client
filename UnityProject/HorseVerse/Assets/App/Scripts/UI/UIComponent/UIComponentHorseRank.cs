using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseRank : UIComponent<UIComponentHorseRank.Entity>
{
    [Serializable]
    public class Entity
    {
        public UIComponentHorseRankList.Entity horseRankList;
        public ButtonComponent.Entity settingBtn;
        public ButtonComponent.Entity backBtn;
    }

    public UIComponentHorseRankList horseRankList;
    public ButtonComponent settingBtn;
    public ButtonComponent backBtn;

    protected override void OnSetEntity()
    {
        horseRankList.SetEntity(this.entity.horseRankList);
        settingBtn.SetEntity(this.entity.settingBtn);
        backBtn.SetEntity(this.entity.backBtn);

    }

}
