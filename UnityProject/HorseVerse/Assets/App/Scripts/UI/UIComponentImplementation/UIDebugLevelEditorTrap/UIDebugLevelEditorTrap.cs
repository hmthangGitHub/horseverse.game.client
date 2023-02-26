using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelEditorTrap : UIComponent<UIDebugLevelEditorTrap.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentToggle.Entity editTargetToggle;
        public UIComponentToggle.Entity editDirectionToggle;
        public UIComponentToggle.Entity editTriggerToggle;
        
        public ButtonComponent.Entity extraBtn;

        public bool isVisibleChildrenPanel;
        public UIDebugLevelEditorTrapToggleList.Entity childrenPanel;
        public ButtonComponent.Entity btnAddChild;
        public ButtonComponent.Entity btnDeleteChild;
    }

    public UIComponentToggle editTargetToggle;
    public UIComponentToggle editDirectionToggle;
    public UIComponentToggle editTriggerToggle;
    public ButtonComponent extraBtn;

    [Space, Header("CHILDREN PANEL")]
    //Extra Panel
    public IsVisibleComponent isVisibleChildrenPanel;
    public UIDebugLevelEditorTrapToggleList childrenPanel;
    public ButtonComponent btnAddChild;
    public ButtonComponent btnDeleteChild;


    protected override void OnSetEntity()
    {
        editTargetToggle.SetEntity(this.entity.editTargetToggle);
        editDirectionToggle.SetEntity(this.entity.editDirectionToggle);
        editTriggerToggle.SetEntity(this.entity.editTriggerToggle);
        extraBtn.SetEntity(this.entity.extraBtn);

        isVisibleChildrenPanel.SetEntity(entity.isVisibleChildrenPanel);
        childrenPanel.SetEntity(entity.childrenPanel);
        btnAddChild.SetEntity(this.entity.btnAddChild);
        btnDeleteChild.SetEntity(this.entity.btnDeleteChild);
    }

    public void SetVisiblePanelChildren(bool active)
    {
        entity.isVisibleChildrenPanel = active;
        isVisibleChildrenPanel.SetEntity(entity.isVisibleChildrenPanel);
    }

    public void SetChildren(UIDebugLevelEditorTrapToggleList.Entity e)
    {
        this.entity.childrenPanel = e;
        childrenPanel.SetEntity(entity.childrenPanel);
    }
}
