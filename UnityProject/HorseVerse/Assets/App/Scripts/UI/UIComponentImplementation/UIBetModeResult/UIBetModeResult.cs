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
        public UIBetModeMyResultPanel.Entity betModeMyResultPanel;

    }

    public UIBetModeResultAnimation Anim;
    public ButtonComponent nextBtn;
    public UIBetModeResultPanel betModeResultPanel;
    public UIBetModeMyResultPanel betModeMyResultPanel;
    

    protected override void OnSetEntity()
    {
        nextBtn.SetEntity(this.entity.nextBtn);
        betModeResultPanel.SetEntity(this.entity.betModeResultPanel);
        betModeMyResultPanel.SetEntity(this.entity.betModeMyResultPanel);
    }

    public async UniTask showResultPanel()
    {
        await betModeResultPanel.In();
    }

    public async UniTask showMyResultPanel()
    {
        await betModeResultPanel.Out();
        await betModeMyResultPanel.In();

    }
}	