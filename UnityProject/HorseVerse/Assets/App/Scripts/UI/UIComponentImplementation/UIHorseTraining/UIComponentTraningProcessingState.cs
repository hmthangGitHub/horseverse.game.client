using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentTraningProcessingState : UIComponent<UIComponentTraningProcessingState.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentCountDownTimer.Entity timer;
    }

    public UIComponentCountDownTimer timer;

    protected override void OnSetEntity()
    {
        timer.SetEntity(this.entity.timer);
    }
}	