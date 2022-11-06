using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelEditorMode : UIComponentEnum<UIDebugLevelEditorMode.Mode>
{
	public enum Mode
	{
		None,
		Block,
		BlockCombo,
		BlockInCombo
	}
}	