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
    }

    public IsVisibleComponent userNameVisible;
    public FormattedTextComponent userName;
    public FormattedTextComponent coin;
    public FormattedTextComponent energy;
    public ButtonComponent backBtn;
    public IsVisibleComponent backBtnVisible;
    public FormattedTextComponent title;
    public UIHeaderAnimation uiHeaderAnimation;
    
    protected override void OnSetEntity()
    {
        userName.SetEntity(this.entity.userName);
        coin.SetEntity(this.entity.coin);
        energy.SetEntity(this.entity.energy, this.entity.maxEnergy);
        title.SetEntity(this.entity.title);
        backBtn.SetEntity(this.entity.backBtn);
        backBtnVisible.SetEntity(this.entity.backBtnVisible);
        userNameVisible.SetEntity(!this.entity.backBtnVisible);
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

    protected override UniTask AnimationIn()
    {
        return uiHeaderAnimation.AnimationIn();
    }

    protected override UniTask AnimationOut()
    {
        return uiHeaderAnimation.AnimationOut();
    }
}
