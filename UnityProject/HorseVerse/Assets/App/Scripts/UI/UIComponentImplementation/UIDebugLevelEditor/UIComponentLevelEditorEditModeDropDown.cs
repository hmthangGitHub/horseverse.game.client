using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentLevelEditorEditModeDropDown : UIComponentDropDownEnum<UIComponentLevelEditorEditModeDropDown.EditMode>
{
	public enum EditMode
	{
		Block,
		Obstacle,
		Coin
	}

	public UIComponentLevelEditorEditModeState editBlockInComboState;
	
	protected override void OnSetEntity()
	{
		editBlockInComboState.SetEntity(this.entity.defaultValue);
		this.entity.onValueChanged += val => editBlockInComboState.SetEntity(val);
		base.OnSetEntity();
	}
}	