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
    private Action<bool> onActiveToggleInternal = ActionUtility.EmptyAction<bool>.Instance;

    private void Awake()
    {
	    toggle.onValueChanged.AddListener(val => onActiveToggleInternal(val));
    }

    protected override void OnSetEntity()
    {
	    toggle.isOn = this.entity.isOn;
	    onActiveToggleInternal = ActionUtility.EmptyAction<bool>.Instance;
	    onActiveToggleInternal += this.entity.onActiveToggle;
    }
}	