using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpSettingInGame : PopupEntity<UIPopUpSettingInGame.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentProgressBar.Entity sfxSlider;
        public UIComponentProgressBar.Entity bgmSlider;
        public UIComponentProgressBar.Entity gfxSlider;
        public ButtonComponent.Entity outerBtn;
        public ButtonComponent.Entity closeBtn;
    }

    public UIComponentProgressBar sfxSlider;
    public UIComponentProgressBar bgmSlider;
    public UIComponentProgressBar gfxSlider;
    public ButtonComponent outerBtn;
    public ButtonComponent closeBtn;

    protected override void OnSetEntity()
    {
        sfxSlider.SetEntity(this.entity.sfxSlider);
        bgmSlider.SetEntity(this.entity.bgmSlider);
        gfxSlider.SetEntity(this.entity.gfxSlider);
        outerBtn.SetEntity(this.entity.outerBtn);
        closeBtn.SetEntity(this.entity.closeBtn);
    }
}	