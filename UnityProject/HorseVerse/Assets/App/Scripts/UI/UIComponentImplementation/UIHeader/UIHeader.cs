using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIHeader : PopupEntity<UIHeader.Entity>
{
    [Serializable]
    public class Entity
    {
        public string userName;
        public long coin;
        public int maxEnergy;
        public int energy;
        public bool backBtnVisible;
        public string title;
        public ButtonComponent.Entity backBtn;
        public ButtonComponent.Entity settingBtn;
        public bool energyVisible;
    }

    public IsVisibleComponent userNameVisible;
    public FormattedTextComponent userName;
    public FormattedTextComponent coin;
    public FormattedTextComponent energy;
    public ButtonComponent backBtn;
    public ButtonComponent settingBtn;
    public IsVisibleComponent backBtnVisible;
    public FormattedTextComponent title;
    public IsVisibleComponent energyVisible;
    
    protected override void OnSetEntity()
    {
        userName.SetEntity(this.entity.userName);
        coin.SetEntity(this.entity.coin);
        energy.SetEntity(this.entity.energy, this.entity.maxEnergy);
        title.SetEntity(this.entity.title);
        backBtn.SetEntity(this.entity.backBtn);
        settingBtn.SetEntity(this.entity.settingBtn);
        backBtnVisible.SetEntity(this.entity.backBtnVisible);
        userNameVisible.SetEntity(!this.entity.backBtnVisible);
        energyVisible.SetEntity(this.entity.energyVisible);
    }

    public void SetVisibleBackBtn(bool visible)
    {
        entity.backBtnVisible = visible;
        backBtnVisible.SetEntity(this.entity.backBtnVisible);
        userNameVisible.SetEntity(!this.entity.backBtnVisible);
    }

    public void SetTitle(string text)
    {
        entity.title = text;
        title.SetEntity(this.entity.title);
    }

    public void SetBackBtnCallBack(Action callback)
    {
        backBtn.SetEntity(callback);
    }
}
