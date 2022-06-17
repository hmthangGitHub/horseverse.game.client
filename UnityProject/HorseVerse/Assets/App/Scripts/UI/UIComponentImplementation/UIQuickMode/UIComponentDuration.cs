using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentDuration : UIComponent<UIComponentDuration.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int duration;
    }

    public FormattedTextComponent text;

    protected override void OnSetEntity()
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, this.entity.duration);
        text.SetEntity(timeSpan.Minutes, timeSpan.Seconds);
    }
}