using System.Collections;
using System.Collections.Generic;
using RuntimeInspectorNamespace;
using UnityEngine;

public class UIDebugMenuObjectModifier : PopupEntity<UIDebugMenuObjectModifier.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public object objectToInspect;
	    public string title;
	    public ButtonComponent.Entity backBtn;
	    public ButtonComponent.Entity closeBtn;
	    public ButtonComponent.Entity okBtn;
    }

    public RuntimeInspector inspector;
    public FormattedTextComponent title;
    public ButtonComponent backBtn;
    public ButtonComponent closeBtn;
    public ButtonComponent okBtn;

    protected override void OnSetEntity()
    {
	    inspector.Inspect(entity.objectToInspect);
	    title.SetEntity(this.entity.title);
	    backBtn.SetEntity(this.entity.backBtn);
	    closeBtn.SetEntity(this.entity.closeBtn);
	    okBtn.SetEntity(this.entity.okBtn);
    }
}	