using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIRaceResultList : PopupEntity<UIRaceResultList.Entity>
{
    [Serializable]
    public class Entity
    {
        public UIComponentHorseResultList.Entity horseList;
        public ButtonComponent.Entity closeBtn;
    }

    public UIComponentHorseResultList horseList;
    public ButtonComponent closeBtn;

    protected override void OnSetEntity()
    {
        horseList.SetEntity(this.entity.horseList);
        this.closeBtn.SetEntity(this.entity.closeBtn);
    }
}
