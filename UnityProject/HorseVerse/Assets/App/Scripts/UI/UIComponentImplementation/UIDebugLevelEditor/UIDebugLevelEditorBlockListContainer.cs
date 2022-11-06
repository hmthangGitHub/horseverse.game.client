using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugLevelEditorBlockListContainer : UIComponent<UIDebugLevelEditorBlockListContainer.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIDebugTrainingBlock.Entity[] blockList;
	    public ButtonComponent.Entity addBtn;
	    public ButtonComponent.Entity closeBtn;
    }

    public UIDebugTrainingBlockList blockList;
    public ButtonComponent addBtn;
    public ButtonComponent closeBtn;
    
    protected override void OnSetEntity()
    {
	    blockList.SetEntity(entity.blockList);
	    addBtn.SetEntity(entity.addBtn);
	    closeBtn.SetEntity(entity.closeBtn);
    }
}	