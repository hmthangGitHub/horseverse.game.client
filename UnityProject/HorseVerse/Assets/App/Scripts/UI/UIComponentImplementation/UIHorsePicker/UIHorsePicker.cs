using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorsePicker : PopupEntity<UIHorsePicker.Entity>
{
    [Serializable]
    public class Entity
    {
        public HorseLoader.Entity horseLoader;
        public ButtonComponent.Entity left;
        public ButtonComponent.Entity right;
        public ButtonComponent.Entity race;
    }

    public HorseLoader horseLoader;
    public ButtonComponent left;
    public ButtonComponent right;
    public ButtonComponent race;

    protected override void OnSetEntity()
    {
        horseLoader.SetEntity(this.entity.horseLoader);
        left.SetEntity(this.entity.left);
        right.SetEntity(this.entity.right);
        race.SetEntity(this.entity.race);
    }
}
