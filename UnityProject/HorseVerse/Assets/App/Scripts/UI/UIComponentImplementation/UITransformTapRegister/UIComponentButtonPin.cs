using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentButtonPin : UIComponent<UIComponentButtonPin.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity btn;
	    public Transform transform;
    }

    public ButtonComponent btn;
    private Camera mainCamera;

    protected override void OnSetEntity()
    {
	    mainCamera = Camera.main;
	    btn.SetEntity(entity.btn);
    }

    private void Update()
    {
	    if (this.entity.transform != default)
	    {
		    transform.position = mainCamera.WorldToScreenPoint(entity.transform.transform.position);
	    }
    }
}	