using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBlockComboType : UIComponentDropDownEnum<UIComponentBlockComboType.BlockComboType>
{
	public enum BlockComboType
	{
		Modular,
		Predefined,
		Custom
	}
}	