using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseTrainingInput : PopupEntity<UIHorseTrainingInput.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity jumpRight;
	    public ButtonComponent.Entity jumpLeft;
    }
    
    public ButtonComponent jumpLeft;
    public ButtonComponent jumpRight;

    protected override void OnSetEntity()
    {
	    jumpRight.SetEntity(this.entity.jumpRight);
	    jumpLeft.SetEntity(this.entity.jumpLeft);
    }
}	