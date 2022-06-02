using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpSettings : PopupEntity<UIPopUpSettings.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentProgressBar.Entity sfxSlider;
        public UIComponentProgressBar.Entity bgmSlider;
        public UIComponentProgressBar.Entity gfxSlider;
        public ButtonComponent.Entity outerBtn;
        public ButtonComponent.Entity closeBtn;
        public ButtonComponent.Entity logOutBtn;
    }

    public UIComponentProgressBar sfxSlider;
    public UIComponentProgressBar bgmSlider;
    public UIComponentProgressBar gfxSlider;
    public ButtonComponent outerBtn;
    public ButtonComponent closeBtn;
    public ButtonComponent logOutBtn;

    protected override void OnSetEntity()
    {
        sfxSlider.SetEntity(this.entity.sfxSlider);
        bgmSlider.SetEntity(this.entity.bgmSlider);
        gfxSlider.SetEntity(this.entity.gfxSlider);
        outerBtn.SetEntity(this.entity.outerBtn);
        closeBtn.SetEntity(this.entity.closeBtn);
        logOutBtn.SetEntity(this.entity.logOutBtn);
    }
}	