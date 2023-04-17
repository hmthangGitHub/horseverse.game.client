using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UIComponentToggleGroup : UIComponent<UIComponentToggleGroup.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public Action<bool> onActiveToggle = _ => { };
    }

    public ToggleGroup toggleGroup;
    public List<Toggle> toggles;

    protected override void OnSetEntity()
    {
        toggles.ForEach(x => x.onValueChanged.AddListener(val => this.entity.onActiveToggle(val)));
    }
}	