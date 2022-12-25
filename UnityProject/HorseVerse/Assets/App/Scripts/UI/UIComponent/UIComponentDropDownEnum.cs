using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentDropDownEnum<T> : UIComponent<UIComponentDropDownEnum<T>.Entity> where T : Enum
{
	[System.Serializable]
    public class Entity
    {
	    public T defaultValue;
	    public Action<T> onValueChanged = ActionUtility.EmptyAction<T>.Instance;
    }

    public UIComponentDropDown dropDown;
    
    protected override void OnSetEntity()
    {
	    dropDown.SetEntity(new UIComponentDropDown.Entity()
	    {
		    options = Enum.GetNames(typeof(T)),
		    defaultValue = (int)(object)this.entity.defaultValue,
		    onValueChanged = val => this.entity.onValueChanged((T)(object)val)
	    });
    }

    private void Reset()
    {
	    dropDown = GetComponent<UIComponentDropDown>();
    }
}	