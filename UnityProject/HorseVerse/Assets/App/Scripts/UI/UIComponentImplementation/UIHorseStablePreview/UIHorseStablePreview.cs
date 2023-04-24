using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStablePreview : PopupEntity<UIHorseStablePreview.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity breedingBtn;
    }
    
    public ButtonComponent breedingBtn;
    
    protected override void OnSetEntity()
    {
	    breedingBtn.SetEntity(entity.breedingBtn);
    }
}	