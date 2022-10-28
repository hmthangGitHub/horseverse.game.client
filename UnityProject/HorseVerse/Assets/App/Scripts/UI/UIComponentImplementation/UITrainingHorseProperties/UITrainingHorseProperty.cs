using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrainingHorseProperty : UIComponent<UITrainingHorseProperty.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string propertyName;
	    public float value;
	    public float maxValue;
	    public Action<float> onValueChange;
    }

    public FormattedTextComponent propertyName;
    public FormattedTextComponent value;
    public Slider slider;
    public ButtonComponent copy;
    
    protected override void OnSetEntity()
    {
	    propertyName.SetEntity(this.entity.propertyName);
	    value.SetEntity(this.entity.value);
	    slider.value = this.entity.value / this.entity.maxValue;
	    slider.onValueChanged.RemoveAllListeners();
	    slider.onValueChanged.AddListener(val =>
	    {
		    value.SetEntity(slider.value * this.entity.maxValue);
		    this.entity.onValueChange?.Invoke(val);
	    });
	    copy.button.onClick.RemoveAllListeners();
	    copy.button.onClick.AddListener(() =>
	    {
		    GUIUtility.systemCopyBuffer = value.text.ToString();
	    });
    }
}	