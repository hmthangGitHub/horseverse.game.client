using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseStableDetailTab : UIComponentEnumToggleGroup<UIComponentHorseStableDetailTab.Tab>
{
	public enum Tab
	{
		Info,
		Age,
		Skill,
		Armor
	}
}	