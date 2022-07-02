using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseRankItem : UIComponent<UIComponentHorseRankItem.Entity>
{
    [Serializable]
    public class Entity
    {
        public string name;
    }
    public new FormattedTextComponent name;

    protected override void OnSetEntity()
    {
        name.SetEntity(this.entity.name);
    }
}
