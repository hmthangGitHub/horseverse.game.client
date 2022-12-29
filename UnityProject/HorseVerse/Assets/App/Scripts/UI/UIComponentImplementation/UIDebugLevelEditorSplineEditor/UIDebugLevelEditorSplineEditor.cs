using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelEditorSplineEditor : UIComponent<UIDebugLevelEditorSplineEditor.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity removeBtn;
        public ButtonComponent.Entity addBtn;
        public ButtonComponent.Entity cancelBtn;
        public ButtonComponent.Entity editBtn;
        public UIComponentSplineEditorMode.Status mode;
        public UIComponentInputField.Entity coinNumber;
    }

    public ButtonComponent removeBtn;
    public ButtonComponent addBtn;
    public ButtonComponent cancelBtn;
    public ButtonComponent editBtn;
    public UIComponentSplineEditorMode mode;
    public UIComponentInputField coinNumber;

    protected override void OnSetEntity()
    {
        removeBtn.SetEntity(this.entity.removeBtn);
        addBtn.SetEntity(this.entity.addBtn);
        cancelBtn.SetEntity(this.entity.cancelBtn);
        editBtn.SetEntity(this.entity.editBtn);
        coinNumber.SetEntity(this.entity.coinNumber);
        mode.SetEntity(this.entity.mode);
    }
}