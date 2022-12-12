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

    public IsVisibleComponent isPlayer;
    public Image image;
    public List<Sprite> sprites;

    protected override void OnSetEntity()
    {
        var sprite = sprites[this.entity.lane - 1];
        this.isPlayer.SetEntity(this.entity.isPlayer);
        this.image.sprite = sprite;
    }
}
