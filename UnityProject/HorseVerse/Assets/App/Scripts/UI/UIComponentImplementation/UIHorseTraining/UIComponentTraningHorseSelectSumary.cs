using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentTraningHorseSelectSumary : UIComponent<UIComponentTraningHorseSelectSumary.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public string horseName;
        public ButtonComponent.Entity selectBtn;
    }

    public FormattedTextComponent horseName;
    public ButtonComponent selectBtn;

    protected override void OnSetEntity()
    {
        horseName.SetEntity(this.entity.horseName);
        selectBtn.SetEntity(this.entity.selectBtn);
    }
}	