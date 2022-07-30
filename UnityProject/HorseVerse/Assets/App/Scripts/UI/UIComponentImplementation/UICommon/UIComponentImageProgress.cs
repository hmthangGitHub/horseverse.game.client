using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentImageProgress : UIComponent<UIComponentImageProgress.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public float progress;
    }

    public Image image;

    protected override void OnSetEntity()
    {
        image.fillAmount = this.entity.progress;
    }

    private void Reset()
    {
        this.image ??= this.gameObject.GetComponent<Image>();
    }
}	