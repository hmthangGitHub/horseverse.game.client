using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentHorseRankBorder : UIComponent<UIComponentHorseRankBorder.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int Rank;
    }

    public Image image;
    public Sprite[] sprites;

    protected override void OnSetEntity()
    {
        if (entity.Rank < sprites.Length)
            image.sprite = sprites[entity.Rank];
    }

}
