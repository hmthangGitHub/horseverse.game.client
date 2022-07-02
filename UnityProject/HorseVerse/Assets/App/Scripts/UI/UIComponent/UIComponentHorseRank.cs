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
    }

    public UIComponentHorseRankList horseRankList;

    protected override void OnSetEntity()
    {
        horseRankList.SetEntity(this.entity.horseRankList);

    }

}
