using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugTrainingBlock : UIComponent<UIDebugTrainingBlock.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string blockName;
	    public ButtonComponent.Entity deleteBtn;
	    public ButtonComponent.Entity selectButtonBtn;
    }
    
    public FormattedTextComponent blockName;
    public ButtonComponent deleteBtn;
    public ButtonComponent selectButtonBtn;
    public IsVisibleComponent isSelected;

    protected override void OnSetEntity()
    {
	    blockName.SetEntity(entity.blockName);
	    deleteBtn.SetEntity(entity.deleteBtn);
	    selectButtonBtn.SetEntity(entity.selectButtonBtn);
	    isSelected.SetEntity(false);
    }

    public void Select(bool isSelect)
    {
	    isSelected.SetEntity(isSelect);
    }
}	