using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseInfo3DView : PopupEntity<UIHorseInfo3DView.Entity>
{
    [Serializable]
    public class Entity
    {
        public HorseLoader.Entity horseLoader;
    }

    public HorseLoader horseLoader;

    protected override void OnSetEntity()
    {
        horseLoader.SetEntity(this.entity.horseLoader);
    }
}
