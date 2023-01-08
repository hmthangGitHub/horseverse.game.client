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
	    public bool isDuplicateBtnVisible;
	    public UIComponentInputField.Entity coinNumber;
	    public UIComponentInputField.Entity blockName;
	    public ButtonComponent.Entity shuffleBtn;
	    public ButtonComponent.Entity leftBtn;
	    public ButtonComponent.Entity rightBtn;
	    public ButtonComponent.Entity deleteBtn;
	    public ButtonComponent.Entity addBtn;
	    public ButtonComponent.Entity duplicateBtn;
    }
    
    public IsVisibleComponent isCoinNumberVisible;
    public IsVisibleComponent isBlockNameVisible;
    public IsVisibleComponent isShuffleBtnVisible;
    public IsVisibleComponent isNavigationBtnVisible;
    public IsVisibleComponent isDeleteBtnVisible;
    public IsVisibleComponent isAddBtnVisible;
    public IsVisibleComponent isDuplicateBtnVisible;
    public UIComponentInputField coinNumber;
    public UIComponentInputField blockName;
    public ButtonComponent shuffleBtn;
    public ButtonComponent leftBtn;
    public ButtonComponent rightBtn;
    public ButtonComponent deleteBtn;
    public ButtonComponent addBtn;
    public ButtonComponent duplicateBtn;
    public RectTransform container;

    protected override void OnSetEntity()
    {
	    isCoinNumberVisible.SetEntity(entity.isCoinNumberVisible);
	    isBlockNameVisible.SetEntity(entity.isBlockNameVisible);
	    isShuffleBtnVisible.SetEntity(entity.isShuffleBtnVisible);
	    isNavigationBtnVisible.SetEntity(entity.isNavigationBtnVisible);
		isDeleteBtnVisible.SetEntity(entity.isDeleteBtnVisible);
	    isAddBtnVisible.SetEntity(entity.isAddBtnVisible);
	    isDuplicateBtnVisible.SetEntity(entity.isDuplicateBtnVisible);
	    coinNumber.SetEntity(entity.coinNumber);
	    blockName.SetEntity(entity.blockName);
	    shuffleBtn.SetEntity(entity.shuffleBtn);
		leftBtn.SetEntity(entity.leftBtn);
		rightBtn.SetEntity(entity.rightBtn);
		deleteBtn.SetEntity(entity.deleteBtn);
		addBtn.SetEntity(entity.addBtn);
		duplicateBtn.SetEntity(entity.duplicateBtn);
    }	

    private void Update()
    {
	    if (entity?.pinTransform != default)
	    {
		    container.transform.position = entity.camera.WorldToScreenPoint(entity.pinTransform.transform.position);
	    }
    }
}