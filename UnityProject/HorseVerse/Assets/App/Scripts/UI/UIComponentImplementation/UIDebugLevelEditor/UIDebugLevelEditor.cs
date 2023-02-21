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
        public ButtonComponent.Entity addTrapBtn;
        public UIDebugLevelEditorMode.Mode editMode;
        public UIDebugLevelEditorBlockListContainer.Entity blockList;
        public UIComponentToggle.Entity editBlockToggle;
        public UIComponentToggle.Entity editObstacleToggle;
        public UIComponentToggle.Entity editCoinToggle;
        public UIComponentToggle.Entity editTrapToggle;
        public UIDebugLevelEditorBlockListContainer.Entity blockComboList;
        public UIDebugLevelEditorBlockListContainer.Entity blockInComboList;
        public IsVisibleComponent.Entity isAddObstacleBtnVisible;
        public IsVisibleComponent.Entity isAddCoinBtnVisible;
        public IsVisibleComponent.Entity isCoinEditorVisible;
        public IsVisibleComponent.Entity isTrapEditorVisible;
        public IsVisibleComponent.Entity isAddFromPresetVisible;
        public IsVisibleComponent.Entity isAddTrapBtnVisible;
        public UIDebugLevelEditorSplineEditor.Entity coinEditor;
        public UIComponentBlockComboType.Entity blockComboType;
        public bool isPopUpVisible;
        public UIDebugLevelEditorTrap.Entity trapEditor;
    }

    public ButtonComponent editBlockBtn;
    public ButtonComponent editBlockComboBtn;
    public ButtonComponent backBtn;
    public ButtonComponent saveBtn;
    public ButtonComponent resetBtn;
    public ButtonComponent addObstacleBtn;
    public ButtonComponent addCoinBtn;
    public ButtonComponent addFromPresetBtn;
    public ButtonComponent addTrapBtn;
    public UIDebugLevelEditorMode editMode;
    public UIDebugLevelEditorBlockListContainer blockList;
    public UIComponentToggle editBlockToggle;
    public UIComponentToggle editObstacleToggle;
    public UIComponentToggle editCoinToggle;
    public UIComponentToggle editTrapToggle;
    public UIDebugLevelEditorBlockListContainer blockComboList;
    public UIDebugLevelEditorBlockListContainer blockInComboList;
    public IsVisibleComponent isPopUpVisible;
    public IsVisibleComponent isAddObstacleBtnVisible;
    public IsVisibleComponent isAddCoinBtnVisible;
    public IsVisibleComponent isAddTrapBtnVisible;
    public IsVisibleComponent isCoinEditorVisible;
    public IsVisibleComponent isTrapEditorVisible;
    public IsVisibleComponent isAddFromPresetVisible;
    public UIDebugLevelEditorSplineEditor coinEditor;
    public UIComponentBlockComboType blockComboType;
    public UIDebugLevelEditorPopUp popUp;
    public UIDebugLevelEditorTrap trapEditor;

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
        addTrapBtn.SetEntity(entity.addTrapBtn);
        blockInComboList.SetEntity(entity.blockInComboList);
        blockComboList.SetEntity(entity.blockComboList);
        isPopUpVisible.SetEntity(entity.isPopUpVisible);

        editBlockToggle.SetEntity(entity.editBlockToggle);
        editObstacleToggle.SetEntity(entity.editObstacleToggle);
        editCoinToggle.SetEntity(entity.editCoinToggle);
        editTrapToggle.SetEntity(entity.editTrapToggle);

        isAddObstacleBtnVisible.SetEntity(entity.isAddObstacleBtnVisible);
        isAddFromPresetVisible.SetEntity(entity.isAddFromPresetVisible);
        isAddCoinBtnVisible.SetEntity(entity.isAddCoinBtnVisible);
        isAddTrapBtnVisible.SetEntity(entity.isAddTrapBtnVisible);
        coinEditor.SetEntity(entity.coinEditor);
        isCoinEditorVisible.SetEntity(entity.isCoinEditorVisible);
        isTrapEditorVisible.SetEntity(entity.isTrapEditorVisible);
        blockComboType.SetEntity(entity.blockComboType);
        trapEditor.SetEntity(entity.trapEditor);
    }

    public void SetPopUpEntity(UIDebugLevelEditorPopUp.Entity popUpEntity)
    {
        entity.isPopUpVisible = true;
        isPopUpVisible.SetEntity(entity.isPopUpVisible);
        popUp.SetEntity(popUpEntity);
    }
}