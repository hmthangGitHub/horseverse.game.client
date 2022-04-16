using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIRaceResultSelf : PopupEntity<UIRaceResultSelf.Entity>
{
    [Serializable]
    public class Entity
    {
        public string name;
        public int top;
        public string speech;
        public ButtonComponent.Entity btnTapAnyWhere;
    }

    public new FormattedTextComponent name;
    public UIComponentHorsePosition top;
    public UIComponentPlayerSpeech speech;
    public ButtonComponent btnTapAnyWhere;

    protected override void OnSetEntity()
    {
        name.SetEntity(this.entity.name);
        top.SetEntity(new UIComponentHorsePosition.Entity()
        {
            top = this.entity.top
        });
        speech.SetEntity(new UIComponentPlayerSpeech.Entity()
        {
            speech = this.entity.speech
        });
        btnTapAnyWhere.SetEntity(this.entity.btnTapAnyWhere);
    }
}
