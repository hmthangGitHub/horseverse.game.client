using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHistoryRecordTimeStamp : UIComponent<UIComponentHistoryRecordTimeStamp.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public long time;
    }

    public FormattedTextComponent time;

    protected override void OnSetEntity()
    {
	    var dateTime = UIComponentCountDownTimer.UnixTimeStampToDateTime(this.entity.time);
	    time.SetEntity(dateTime.TimeOfDay, dateTime.Date.ToShortDateString());
    }

    public void SetEntity(long time)
    {
	    SetEntity(new Entity()
	    {
		    time = time
	    });
    }
}	