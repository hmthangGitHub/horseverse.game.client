using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetAmouth : UIComponent<UIComponentBetAmouth.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public int betAmouth;
    }

    public FormattedTextComponent betAmouth;

    protected override void OnSetEntity()
    {
        betAmouth.SetEntity(this.entity.betAmouth);
    }
}	