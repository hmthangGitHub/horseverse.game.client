using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorse3DView : PopupEntity<UIHorse3DView.Entity>
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
