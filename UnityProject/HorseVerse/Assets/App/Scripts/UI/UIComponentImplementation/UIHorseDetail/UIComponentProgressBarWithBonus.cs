using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentProgressBarWithBonus : UIComponent<UIComponentProgressBarWithBonus.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentProgressBar.Entity progressBar;
        public float bonus;
    }

    public UIComponentProgressBar progressBar;
    public FormattedTextComponent bonus;

    protected override void OnSetEntity()
    {
        progressBar.SetEntity(this.entity.progressBar);
        bonus.SetEntity(this.entity.bonus);
    }
}	