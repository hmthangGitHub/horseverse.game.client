using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRawImage : UIComponent<UIRawImage.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public Texture sprite;
    }

    public RawImage img;

    protected override void OnSetEntity()
    {
        img.texture = this.entity.sprite;
        if (img.texture == default) img.enabled = false;
        else img.enabled = true;
    }
}
