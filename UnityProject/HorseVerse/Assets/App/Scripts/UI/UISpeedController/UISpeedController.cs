using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UISpeedController : PopupEntity<UISpeedController.Entity>
{
    [Serializable]
    public class Entity
    {
        public ButtonComponent.Entity pause;
        public ButtonComponent.Entity normal;
        public ButtonComponent.Entity fast;
        public ButtonComponent.Entity skip;
    }

    public ButtonComponent pause;
    public ButtonComponent normal;
    public ButtonComponent fast;
    public ButtonComponent skip;

    protected override void OnSetEntity()
    {
        pause.SetEntity(this.entity.pause);
        normal.SetEntity(this.entity.normal);
        fast.SetEntity(this.entity.fast);
        skip.SetEntity(this.entity.skip);
    }
}
