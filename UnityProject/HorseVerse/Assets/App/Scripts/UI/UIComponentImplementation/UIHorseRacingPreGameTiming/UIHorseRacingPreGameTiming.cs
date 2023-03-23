using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHorseRacingPreGameTiming : PopupEntity<UIHorseRacingPreGameTiming.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public Action<UIHorseRacingTimingType.TimingType> timingType = ActionUtility.EmptyAction<UIHorseRacingTimingType.TimingType>.Instance;
    }

	private Tween timingTween;
	public ButtonComponent tapBtn;
    public RectTransform[] markers;
    public RectTransform timingBar;
    public float roundTime;
    public UIHorseRacingTimingType timingType;

    public UnityEngine.UI.Slider slider;
    
    protected override void OnSetEntity()
    {
	    timingType.SetEntity(UIHorseRacingTimingType.TimingType.None);
	    tapBtn.SetEntity(StopTimming);
    }

    public void StartTiming()
    {
	    timingType.SetEntity(UIHorseRacingTimingType.TimingType.None);
	    timingTween = DOTween.To(val =>
	                         {
		                         slider.value = val;
	                         }, 0.0f, 1.0f, roundTime)
	                         .SetEase(Ease.Linear)
	                         .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopTimming()
    {
	    if (timingTween == default) return;
	    timingTween.Kill();
	    timingTween = default;
	    var markerIndex = markers.ToList()
	                             .FindLastIndex(x => IsInRange(slider.value, GetValueRange(x)));
	    var timingTypeResult = (UIHorseRacingTimingType.TimingType)(markerIndex + 2);
	    timingType.SetEntity(timingTypeResult);
	    this.entity.timingType.Invoke(timingTypeResult);
    }

    private Vector2 GetValueRange(RectTransform marker)
    {
	    return new Vector2((marker.anchoredPosition.x - marker.rect.width / 2) / timingBar.rect.width,
		    (marker.anchoredPosition.x + marker.rect.width / 2) / timingBar.rect.width);
    }

    private bool IsInRange(float value,
                           Vector2 range)
    {
	    return value >= range.x && value <= range.y;
    }
}	