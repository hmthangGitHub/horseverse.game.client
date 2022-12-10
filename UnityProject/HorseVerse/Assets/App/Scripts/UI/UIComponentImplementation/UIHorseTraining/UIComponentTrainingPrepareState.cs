using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentTrainingPrepareState : UIComponent<UIComponentTrainingPrepareState.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity toTraningBtn;
        public int traningCost;
        //public UIComponentHorseTraningMapSelection.Entity mapSelection;
    }

    public ButtonComponent toTraningBtn;
    public FormattedTextComponent traningCost;
    //public UIComponentHorseTraningMapSelection mapSelection;

    protected override void OnSetEntity()
    {
        toTraningBtn.SetEntity(this.entity.toTraningBtn);
        traningCost.SetEntity(this.entity.traningCost);
        //mapSelection.SetEntity(this.entity.mapSelection);
    }
}	