using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseCardSelectToBreed : PopupEntity<UIHorseCardSelectToBreed.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentHorseBreedingCard.Entity[] horseBreedingList;
    }
    
    public UIComponentHorseBreedingCardList horseBreedingList;

    protected override void OnSetEntity()
    {
	    horseBreedingList.SetEntity(entity.horseBreedingList);
    }
}	