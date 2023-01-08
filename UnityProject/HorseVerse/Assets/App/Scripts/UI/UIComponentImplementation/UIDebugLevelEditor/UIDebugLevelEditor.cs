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
        public ButtonComponent.Entity addFromPresetBtn;
        public UIDebugLevelEditorMode.Mode editMode;
        public UIDebugLevelEditorBlockListContainer.Entity blockList;
        public UIComponentToggle.Entity editBlockToggle;
        public UIComponentToggle.Entity editObstacleToggle;
        public UIComponentToggle.Entity editCoinToggle;
        public UIDebugLevelEditorBlockListContainer.Entity blockComboList;
        public UIDebugLevelEditorBlockListContainer.Entity blockInComboList;
        public IsVisibleComponent.Entity isAddObstacleBtnVisible;
        public IsVisibleComponent.Entity isAddCoinBtnVisible;
        public IsVisibleComponent.Entity isCoinEditorVisible;
        public IsVisibleComponent.Entity isAddFromPresetVisible;
        public UIDebugLevelEditorSplineEditor.Entity coinEditor;
        public UIComponentBlockComboType.Entity blockComboType;
        public bool isPopUpVisible;
    }

    public ButtonComponent editBlockBtn;
    public ButtonComponent editBlockComboBtn;
    public ButtonComponent backBtn;
    public ButtonComponent saveBtn;
    public ButtonComponent resetBtn;
    public ButtonComponent addObstacleBtn;
    public ButtonComponent addCoinBtn;
    public ButtonComponent addFromPresetBtn;
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
    public IsVisibleComponent isCoinEditorVisible;
    public IsVisibleComponent isAddFromPresetVisible;
    public UIDebugLevelEditorSplineEditor coinEditor;
    public UIComponentBlockComboType blockComboType;
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
        addFromPresetBtn.SetEntity(entity.addFromPresetBtn);
        blockInComboList.SetEntity(entity.blockInComboList);
        blockComboList.SetEntity(entity.blockComboList);
        isPopUpVisible.SetEntity(entity.isPopUpVisible);
        editBlockToggle.SetEntity(entity.editBlockToggle);
        editObstacleToggle.SetEntity(entity.editObstacleToggle);
        editCoinToggle.SetEntity(entity.editCoinToggle);
        isAddObstacleBtnVisible.SetEntity(entity.isAddObstacleBtnVisible);
        isAddFromPresetVisible.SetEntity(entity.isAddFromPresetVisible);
        isAddCoinBtnVisible.SetEntity(entity.isAddCoinBtnVisible);
        coinEditor.SetEntity(entity.coinEditor);
        isCoinEditorVisible.SetEntity(entity.isCoinEditorVisible);
        blockComboType.SetEntity(entity.blockComboType);
    }

    public void SetPopUpEntity(UIDebugLevelEditorPopUp.Entity popUpEntity)
    {
        entity.isPopUpVisible = true;
        isPopUpVisible.SetEntity(entity.isPopUpVisible);
        popUp.SetEntity(popUpEntity);
    }
}