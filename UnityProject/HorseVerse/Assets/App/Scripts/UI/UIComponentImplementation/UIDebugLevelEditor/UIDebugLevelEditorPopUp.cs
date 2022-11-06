using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelEditorPopUp : UIComponent<UIDebugLevelEditorPopUp.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIDebugLevelEditorPopUpMode.Mode popUpMode;
	    public string confirmationText;
	    public UIComponentInputField.Entity inputField;
	    public ButtonComponent.Entity okBtn;
	    public ButtonComponent.Entity closeBtn;
    }
    
    public UIDebugLevelEditorPopUpMode popUpMode;
    public FormattedTextComponent confirmationText;
    public UIComponentInputField inputField;
    public ButtonComponent okBtn;
    public ButtonComponent  closeBtn;
    
    protected override void OnSetEntity()
    {
	    popUpMode.SetEntity(this.entity.popUpMode);
	    confirmationText.SetEntity(this.entity.confirmationText);
	    inputField.SetEntity(this.entity.inputField);
	    okBtn.SetEntity(this.entity.okBtn);
	    closeBtn.SetEntity(this.entity.closeBtn);
    }
}	