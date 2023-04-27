using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseBreedingCard : UIComponent<UIComponentHorseBreedingCard.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentHorseElement.Element element;
	    public string horseName;
	    public int breedCount;
	    public int maxBreedCount;
	    public bool isLock;
	    public bool isCountDown;
	    public UIComponentCountDownTimer.Entity countDown;
	    public ButtonComponent.Entity selectBtn;
    }
    
    public UIComponentHorseElement element;
    public FormattedTextComponent horseName;
    public FormattedTextComponent breedCount;
    public IsVisibleComponent isLock;
    public ButtonComponent selectBtn;
    public IsVisibleComponent isCountDown;
    public UIComponentCountDownTimer countDown;
    
    protected override void OnSetEntity()
    {
	    element.SetEntity(entity.element);
	    horseName.SetEntity(entity.horseName);
	    breedCount.SetEntity(entity.breedCount, entity.maxBreedCount);
	    isLock.SetEntity(entity.isLock);
	    selectBtn.SetEntity(entity.selectBtn);
	    isCountDown.SetEntity(entity.isCountDown);
		countDown.SetEntity(entity.countDown);
    }
}	