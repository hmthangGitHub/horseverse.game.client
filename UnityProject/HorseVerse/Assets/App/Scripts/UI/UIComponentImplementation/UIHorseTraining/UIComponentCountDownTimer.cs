using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FormattedTextComponent))]
public class UIComponentCountDownTimer : UIComponent<UIComponentCountDownTimer.Entity>
{
    [Serializable]
    public class Entity
    {
        [SerializeField]
        public Action outDatedEvent;
        public int utcEndTimeStamp;
    }

    public FormattedTextComponent timeLeftText;

    public enum Format
    {
        DD_HH_MM_SS,
        SS,
    }

    public Format format = Format.DD_HH_MM_SS;
    private DateTime endTimeStampDateTime;
    private float updateInterval = 1.0f;
    private float currentTime = 0.0f;
    private bool outOfDate = false;

    protected override void OnSetEntity()
    {
        timeLeftText.format = GetFormat(format);
        endTimeStampDateTime = UnixTimeStampToDateTime(this.entity.utcEndTimeStamp);
        outOfDate = false;
        SetDateTimeLeft();
    }

    private string GetFormat(Format format)
    {
        return format switch
        {
            Format.DD_HH_MM_SS => "{0:00}:{1:00}:{2:00}:{3:00}",
            Format.SS => "{0:00}",
            _ => "{0:00}"
        };
    }

    private void Update()
    {
        if (!outOfDate && this.entity != default)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= updateInterval)
            {
                currentTime -= updateInterval;
                SetDateTimeLeft();
            }
        }
    }

    private void SetDateTimeLeft()
    {
        TimeSpan timeLeft = GetTimeLeft();
        if (GetTimeLeft().TotalSeconds <= 0)
        {
            outOfDate = true;
            this.entity.outDatedEvent?.Invoke();
            timeLeft = new TimeSpan(0, 0, 0, 0);
        }
        switch (format)
        {
            case Format.DD_HH_MM_SS:
                timeLeftText.SetEntity(timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
                break;
            case Format.SS:
                timeLeftText.SetEntity(timeLeft.TotalSeconds);
                break;
            default:
                break;
        }
    }

    private TimeSpan GetTimeLeft()
    {
        return (endTimeStampDateTime - DateTime.UtcNow);
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }

    void Reset()
    {
        timeLeftText ??= this.GetComponent<FormattedTextComponent>();
    }
}	