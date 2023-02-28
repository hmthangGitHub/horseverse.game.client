using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetModeResult : PopupEntity<UIBetModeResult.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity nextBtn;
        public UIBetModeResultPanel.Entity betModeResultPanel;

    }

    public ButtonComponent nextBtn;
    public UIBetModeResultPanel betModeResultPanel;
    

    protected override void OnSetEntity()
    {
        nextBtn.SetEntity(this.entity.nextBtn);
        betModeResultPanel.SetEntity(this.entity.betModeResultPanel);
    }
}	