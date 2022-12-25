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
        public ButtonComponent.Entity addObstacleBtn;
        public ButtonComponent.Entity addCoinBtn;
        public UIDebugLevelEditorMode.Mode editMode;
        public UIDebugLevelEditorBlockListContainer.Entity blockList;
        public UIComponentToggle.Entity editBlockToggle;
        public UIComponentToggle.Entity editObstacleToggle;
        public UIComponentToggle.Entity editCoinToggle;
        public UIDebugLevelEditorBlockListContainer.Entity blockComboList;
        public UIDebugLevelEditorBlockListContainer.Entity blockInComboList;
        public IsVisibleComponent.Entity isAddObstacleBtnVisible;
        public IsVisibleComponent.Entity isAddCoinBtnVisible;
        public bool isPopUpVisible;
    }

    public ButtonComponent editBlockBtn;
    public ButtonComponent editBlockComboBtn;
    public ButtonComponent backBtn;
    public ButtonComponent saveBtn;
    public ButtonComponent resetBtn;
    public ButtonComponent addObstacleBtn;
    public ButtonComponent addCoinBtn;
    public UIDebugLevelEditorMode editMode;
    public UIDebugLevelEditorBlockListContainer blockList;
    public UIComponentToggle editBlockToggle;
    public UIComponentToggle editObstacleToggle;
    public UIComponentToggle editCoinToggle;
    public UIDebugLevelEditorBlockListContainer blockComboList;
    public UIDebugLevelEditorBlockListContainer blockInComboList;
    public IsVisibleComponent isPopUpVisible;
    public IsVisibleComponent isAddObstacleBtnVisible;
    public IsVisibleComponent isAddCoinBtnVisible;
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
        addObstacleBtn.SetEntity(entity.addObstacleBtn);
        addCoinBtn.SetEntity(entity.addCoinBtn);
        blockInComboList.SetEntity(entity.blockInComboList);
        blockComboList.SetEntity(entity.blockComboList);
        isPopUpVisible.SetEntity(entity.isPopUpVisible);
        editBlockToggle.SetEntity(entity.editBlockToggle);
        editObstacleToggle.SetEntity(entity.editObstacleToggle);
        editCoinToggle.SetEntity(entity.editCoinToggle);
        isAddObstacleBtnVisible.SetEntity(entity.isAddCoinBtnVisible);
        isAddCoinBtnVisible.SetEntity(entity.isAddCoinBtnVisible);
    }

    public void SetPopUpEntity(UIDebugLevelEditorPopUp.Entity popUpEntity)
    {
        entity.isPopUpVisible = true;
        isPopUpVisible.SetEntity(entity.isPopUpVisible);
        popUp.SetEntity(popUpEntity);
    }
}