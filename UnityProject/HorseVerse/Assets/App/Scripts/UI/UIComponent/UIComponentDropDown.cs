using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TMP_Dropdown))]
public class UIComponentDropDown : UIComponent<UIComponentDropDown.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string[] options;
	    public int defaultValue;
	    public Action<int> onValueChanged = ActionUtility.EmptyAction<int>.Instance;
    }

    private void Awake()
    {
	    dropDown.onValueChanged.AddListener(val => this.onValueChange.Invoke(val));
    }

    public TMP_Dropdown dropDown;
    private Action<int> onValueChange = ActionUtility.EmptyAction<int>.Instance;

    protected override void OnSetEntity()
    {
	    onValueChange = ActionUtility.EmptyAction<int>.Instance;
	    dropDown.options = this.entity.options.Select(x => new TMP_Dropdown.OptionData()
	                           {
		                           text = x
	                           })
	                           .ToList();
	    dropDown.value = this.entity.defaultValue;
	    onValueChange += this.entity.onValueChanged;
    }

    private void Reset()
    {
	    dropDown = GetComponent<TMP_Dropdown>();
    }
}	