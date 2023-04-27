using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBreedSlotState : UIComponentEnum<UIBreedSlotState.State>
{
	public enum State
	{
		Empty,
		Breeding,
		Locked,
		CheckingFoal
	}
}	