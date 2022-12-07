using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImage : UIComponent<UIImage.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public Sprite sprite;
    }

    public Image img;

    protected override void OnSetEntity()
    {
        img.sprite = this.entity.sprite;
        if (img.sprite == default) img.enabled = false;
        else img.enabled = true;
    }
}
