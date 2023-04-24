using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStableBreedSlot : PopupEntity<UIHorseStableBreedSlot.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIBreedSlot.Entity[] breedSlotList;
    }

    public UIBreedSlotList breedSlotList;
    
    protected override void OnSetEntity()
    {
	    breedSlotList.SetEntity(entity.breedSlotList);
    }
}	