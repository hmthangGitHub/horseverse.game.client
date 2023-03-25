using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIComponentHoldImageBehavior : UIComponent<UIComponentHoldImageBehavior.Entity>, IPointerUpHandler, IPointerDownHandler, IUpdateSelectedHandler
{
	[System.Serializable]
    public class Entity
    {
	    public Action onDown = ActionUtility.EmptyAction.Instance;
	    public Action onUp = ActionUtility.EmptyAction.Instance;
    }

    private bool isPressed;
     
    public void OnUpdateSelected(BaseEventData data)
    {
	    if (isPressed)
	    {
	    }
    }
    
    public void OnPointerDown(PointerEventData data)
    {
	    isPressed = true;
	    this.entity.onDown.Invoke();
    }
    
    public void OnPointerUp(PointerEventData data)
    {
	    isPressed = false;
	    this.entity.onUp.Invoke();
    }
    
    protected override void OnSetEntity()
    {
    }
}	