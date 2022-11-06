using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelDesignBlockTransformPin : PopupEntity<UIDebugLevelDesignBlockTransformPin.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public Camera camera;
	    public Transform pinTransform;
	    public bool isCoinNumberVisible;
	    public bool isBlockNameVisible;
	    public bool isShuffleBtnVisible;
	    public bool isNavigationBtnVisible;
	    public bool isDeleteBtnVisible;
	    public bool isAddBtnVisible;
	    public UIComponentInputField.Entity coinNumber;
	    public UIComponentInputField.Entity blockName;
	    public ButtonComponent.Entity shuffleBtn;
	    public ButtonComponent.Entity leftBtn;
	    public ButtonComponent.Entity rightBtn;
	    public ButtonComponent.Entity deleteBtn;
	    public ButtonComponent.Entity addBtn;
    }
    
    public IsVisibleComponent isCoinNumberVisible;
    public IsVisibleComponent isBlockNameVisible;
    public IsVisibleComponent isShuffleBtnVisible;
    public IsVisibleComponent isNavigationBtnVisible;
    public IsVisibleComponent isDeleteBtnVisible;
    public IsVisibleComponent isAddBtnVisible;
    public UIComponentInputField coinNumber;
    public UIComponentInputField blockName;
    public ButtonComponent shuffleBtn;
    public ButtonComponent leftBtn;
    public ButtonComponent rightBtn;
    public ButtonComponent deleteBtn;
    public ButtonComponent addBtn;
    public RectTransform container;

    protected override void OnSetEntity()
    {
	    isCoinNumberVisible.SetEntity(entity.isCoinNumberVisible);
	    isBlockNameVisible.SetEntity(entity.isBlockNameVisible);
	    isShuffleBtnVisible.SetEntity(entity.isShuffleBtnVisible);
	    isNavigationBtnVisible.SetEntity(entity.isNavigationBtnVisible);
		isDeleteBtnVisible.SetEntity(entity.isDeleteBtnVisible);
	    isAddBtnVisible.SetEntity(entity.isAddBtnVisible);
	    coinNumber.SetEntity(entity.coinNumber);
	    blockName.SetEntity(entity.blockName);
	    shuffleBtn.SetEntity(entity.shuffleBtn);
		leftBtn.SetEntity(entity.leftBtn);
		rightBtn.SetEntity(entity.rightBtn);
		deleteBtn.SetEntity(entity.deleteBtn);
		addBtn.SetEntity(entity.addBtn);
    }	

    private void Update()
    {
	    if (entity?.pinTransform != default)
	    {
		    container.transform.position = entity.camera.WorldToScreenPoint(entity.pinTransform.transform.position);
		    // var screenPoint = Camera.main.WorldToScreenPoint(entity.pinTransform.position);
		    // RectTransformUtility.ScreenPointToLocalPointInRectangle(container.RectTransform(), screenPoint, Camera.current, out var localPosition);
		    // container.anchoredPosition = localPosition;    
	    }
    }
}