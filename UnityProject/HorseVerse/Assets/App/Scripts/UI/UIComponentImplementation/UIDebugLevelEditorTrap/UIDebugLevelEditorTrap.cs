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
    }

    public UIComponentToggle editTargetToggle;
    public UIComponentToggle editDirectionToggle;
    public UIComponentToggle editTriggerToggle;
    public ButtonComponent extraBtn;

    protected override void OnSetEntity()
    {
        editTargetToggle.SetEntity(this.entity.editTargetToggle);
        editDirectionToggle.SetEntity(this.entity.editDirectionToggle);
        editTriggerToggle.SetEntity(this.entity.editTriggerToggle);
        extraBtn.SetEntity(this.entity.extraBtn);
    }
}
