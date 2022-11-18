using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugMasterEditor : PopupEntity<UIDebugMasterEditor.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIDebugMasterColumnList.Entity masterColumnList;
	    public string title;
	    public ButtonComponent.Entity closeBtn;
	    public ButtonComponent.Entity resetBtn;
	    public ButtonComponent.Entity saveBtn;
	    public ButtonComponent.Entity exportBtn;
	    public ButtonComponent.Entity importBtn;
    }

    public UIDebugMasterColumnList masterColumnList;
    public FormattedTextComponent title;
    public ButtonComponent closeBtn;
    public ButtonComponent resetBtn;
    public ButtonComponent saveBtn;
    public ButtonComponent exportBtn;
    public ButtonComponent importBtn;
    
    protected override void OnSetEntity()
    {
	    masterColumnList.SetEntity(this.entity.masterColumnList);
		title.SetEntity(this.entity.title);
		closeBtn.SetEntity(this.entity.closeBtn);
		resetBtn.SetEntity(this.entity.resetBtn);
		saveBtn.SetEntity(this.entity.saveBtn);
		exportBtn.SetEntity(this.entity.exportBtn);
		importBtn.SetEntity(this.entity.importBtn);
    }
}	