using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelEditorTrapToggleItem : UIComponent<UIDebugLevelEditorTrapToggleItem.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentToggle.Entity toggle;
        public string title;
    }

    public UIComponentToggle toggle;
    public FormattedTextComponent title;

    protected override void OnSetEntity()
    {
        toggle.SetEntity(this.entity.toggle);
        title.SetEntity(this.entity.title);
    }
}
