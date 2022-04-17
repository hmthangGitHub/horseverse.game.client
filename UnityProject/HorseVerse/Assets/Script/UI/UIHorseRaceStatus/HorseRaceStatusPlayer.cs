using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorseRaceStatusPlayer : UIComponent<HorseRaceStatusPlayer.Entity>
{
    [Serializable]
    public class Entity
    {
        public int lane;
        public bool isPlayer;
    }

    public FormattedTextComponent index;
    public IsVisibleComponent isPlayer;
    public Image image;

    protected override void OnSetEntity()
    {
        this.index.SetEntity(this.entity.lane);
        this.isPlayer.SetEntity(this.entity.isPlayer);
        this.image.color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f) , 1.0f);
    }
}
