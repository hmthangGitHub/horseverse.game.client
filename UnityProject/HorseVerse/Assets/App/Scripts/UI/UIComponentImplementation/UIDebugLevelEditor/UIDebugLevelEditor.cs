using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelEditor : PopupEntity<UIDebugLevelEditor.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity editBlockBtn;
	    public ButtonComponent.Entity editBlockComboBtn;
	    public ButtonComponent.Entity backBtn;
	    public ButtonComponent.Entity saveBtn;
	    public ButtonComponent.Entity resetBtn;
	    public UIDebugLevelEditorMode.Mode editMode;
	    public UIDebugLevelEditorBlockListContainer.Entity blockList;
	    public UIDebugLevelEditorBlockListContainer.Entity blockComboList;
	    public UIDebugLevelEditorBlockListContainer.Entity blockInComboList;
	    public bool isPopUpVisible;
    }
    
    public ButtonComponent editBlockBtn;
    public ButtonComponent editBlockComboBtn;
    public ButtonComponent backBtn;
    public ButtonComponent saveBtn;
    public ButtonComponent resetBtn;
    public UIDebugLevelEditorMode editMode;
    public UIDebugLevelEditorBlockListContainer blockList;
    public UIDebugLevelEditorBlockListContainer blockComboList;
    public UIDebugLevelEditorBlockListContainer blockInComboList;
    public IsVisibleComponent isPopUpVisible;
    public UIDebugLevelEditorPopUp popUp;

    protected override void OnSetEntity()
    {
		editBlockBtn.SetEntity(entity.editBlockBtn);
		editBlockComboBtn.SetEntity(entity.editBlockComboBtn);
		editMode.SetEntity(entity.editMode);
		backBtn.SetEntity(entity.backBtn);
		saveBtn.SetEntity(entity.saveBtn);
		resetBtn.SetEntity(entity.resetBtn);
		blockList.SetEntity(entity.blockList);
		blockInComboList.SetEntity(entity.blockInComboList);
		blockComboList.SetEntity(entity.blockComboList);
		isPopUpVisible.SetEntity(entity.isPopUpVisible);
    }

    public void SetPopUpEntity(UIDebugLevelEditorPopUp.Entity popUpEntity)
    {
	    entity.isPopUpVisible = true;
	    isPopUpVisible.SetEntity(entity.isPopUpVisible);
	    popUp.SetEntity(popUpEntity);
    }
}