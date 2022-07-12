using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultTimeAndPricePool : PopupEntity<UIResultTimeAndPricePool.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public float totalSeconds;
        public int prizePool;
    }

    public UIComponentTimeSpan totalSeconds;
    public FormattedTextComponent prizePool;

    protected override void OnSetEntity()
    {
        totalSeconds.SetEntity(this.entity.totalSeconds);
        prizePool.SetEntity(this.entity.prizePool);
    }
}	