using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FormattedTextComponent))]
public class UIComponentTimeSpan : UIComponent<UIComponentTimeSpan.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public float totalSecond;
    }

    public enum Format
    {
        None,
        DD_HH_MM_SS,
        HH_MM_SS_MM,
        DD_HH_MM_SS_MM,
        SS,
    }

    public Format format = Format.DD_HH_MM_SS;
    public FormattedTextComponent timeLeftText;
    private object[] timeParams;

    protected override void OnSetEntity()
    {
        timeLeftText.format = GetFormat(format);
        var timeLeft = TimeSpan.FromSeconds(this.entity.totalSecond);
        switch (format)
        {
            case Format.DD_HH_MM_SS_MM:
            case Format.DD_HH_MM_SS:
            case Format.HH_MM_SS_MM:
                timeParams ??= new object[5];
                timeParams[0] = timeLeft.Days;
                timeParams[1] = timeLeft.Hours;
                timeParams[2] = timeLeft.Minutes;
                timeParams[3] = timeLeft.Seconds;
                timeParams[4] = timeLeft.Milliseconds / 10;
                timeLeftText.SetWithArrayEntity(timeParams);
                break;
            case Format.SS:
                timeLeftText.SetEntity(timeLeft.TotalSeconds);
                break;
            default:
                break;
        }
    }

    private string GetFormat(Format format)
    {
        return format switch
        {
            Format.DD_HH_MM_SS => "{0:00}:{1:00}:{2:00}:{3:00}",
            Format.DD_HH_MM_SS_MM => "{0:00}:{1:00}:{2:00}:{3:00}.{4:00}",
            Format.HH_MM_SS_MM => "{1:00}:{2:00}:{3:00}.{4:00}",
            Format.SS => "{0:00}",
            _ => "{0:00}"
        };
    }

    public void SetEntity(float totalSecond)
    {
        this.entity ??= new UIComponentTimeSpan.Entity();
        this.entity.totalSecond = totalSecond;
        OnSetEntity();
    }

    public void Reset()
    {
        timeLeftText = this.GetComponent<FormattedTextComponent>();
    }
}	