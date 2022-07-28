using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIComponentProgressBar : UIComponent<UIComponentProgressBar.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public float progress;
    }

    public Slider slider;

    protected override void OnSetEntity()
    {
        slider.value = this.entity.progress;
    }

    public void SetProgress(float progress)
    {
        this.entity.progress = progress;
        OnSetEntity();
    }

    void Reset()
    {
        slider ??= this.GetComponent<Slider>();
    }
}	