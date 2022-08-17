using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentToggle : UIComponent<UIComponentToggle.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public bool isOn;
		public Action<bool> onActiveToggle = ActionUtility.EmptyAction<bool>.Instance;
    }

    public Toggle toggle;

    protected override void OnSetEntity()
    {
	    toggle.isOn = this.entity.isOn;
	    toggle.onValueChanged.RemoveAllListeners();
	    toggle.onValueChanged.AddListener(val => this.entity.onActiveToggle(val));
    }
}	