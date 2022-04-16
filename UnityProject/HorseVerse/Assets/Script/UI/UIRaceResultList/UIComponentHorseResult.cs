using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentHorseResult : UIComponent<UIComponentHorseResult.Entity>
{
    [Serializable]
    public class Entity
    {
        public int top;
        public int lane;
        public float time;
        public string name;
        public bool isPlayer;
    }

    public FormattedTextComponent top;
    public FormattedTextComponent lane;
    public FormattedTextComponent time;
    public new FormattedTextComponent name;
    public IsVisibleComponent isPlayer;

    public Image[] images;

    protected override void OnSetEntity()
    {
        this.top.SetEntity(this.entity.top);
        this.lane.SetEntity(this.entity.lane);
        this.time.SetEntity(this.entity.time);
        this.name.SetEntity(this.entity.name);
        this.isPlayer.SetEntity(this.entity.isPlayer);

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
        }
    }
}
