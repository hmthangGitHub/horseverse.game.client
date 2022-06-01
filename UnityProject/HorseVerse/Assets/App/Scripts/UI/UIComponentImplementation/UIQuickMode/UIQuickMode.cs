using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuickMode : PopupEntity<UIQuickMode.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity backBtn;
        public FormattedTextComponent.Entity energy;
        public ButtonComponent.Entity findMatchBtn;
        public ButtonComponent.Entity cancelMatchBtn;
        public IsVisibleComponent.Entity findMatchBtnVisible;
        public IsVisibleComponent.Entity cancelMatchBtnVisible;
        public long findMatchTimer;
    }

    public ButtonComponent backBtn;
    public FormattedTextComponent energy;
    public ButtonComponent findMatchBtn;
    public ButtonComponent cancelMatchBtn;
    public IsVisibleComponent findMatchBtnVisible;
    public IsVisibleComponent cancelMatchBtnVisible;
    public FormattedTextComponent findMatchTimer;

    protected override void OnSetEntity()
    {
        backBtn.SetEntity(this.entity.backBtn);
        energy.SetEntity(this.entity.energy);
        findMatchBtn.SetEntity(this.entity.findMatchBtn);
        cancelMatchBtn.SetEntity(this.entity.cancelMatchBtn);
        findMatchTimer.SetEntity(this.entity.findMatchTimer);
    }
}	