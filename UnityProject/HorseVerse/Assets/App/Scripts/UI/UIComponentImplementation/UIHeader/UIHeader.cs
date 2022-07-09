using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeader : PopupEntity<UIHeader.Entity>
{
    [Serializable]
    public class Entity
    {
        public string userName;
        public int coin;
        public int maxEnergy;
        public int energy;
        public bool backBtnVisible;
        public ButtonComponent.Entity backBtn;
    }

    public FormattedTextComponent userName;
    public FormattedTextComponent coin;
    public FormattedTextComponent energy;
    public ButtonComponent backBtn;
    public IsVisibleComponent backBtnVisible;

    protected override void OnSetEntity()
    {
        userName.SetEntity(this.entity.userName);
        coin.SetEntity(this.entity.coin);
        energy.SetEntity(this.entity.energy, this.entity.maxEnergy);
        backBtn.SetEntity(this.entity.backBtn);
        backBtnVisible.SetEntity(this.entity.backBtnVisible);
    }

    public void SetVisibleBackBtn(bool visible)
    {
        entity.backBtnVisible = visible;
        backBtnVisible.SetEntity(this.entity.backBtnVisible);
    }

    public void SetBackBtnCallBack(Action callback)
    {
        backBtn.SetEntity(callback);
    } 
}
