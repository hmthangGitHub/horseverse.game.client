using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
    public FormattedTextComponent timer;

    public UnityEngine.UI.Slider slider;
    private UIHorseRacingTimingType.TimingType result = UIHorseRacingTimingType.TimingType.None;
    
    protected override void OnSetEntity()
    {
	    timingType.SetEntity(UIHorseRacingTimingType.TimingType.None);
	    tapBtn.SetEntity(StopTimming);
    }

    public void StartTiming()
    {
	    result = default;
	    timingType.SetEntity(UIHorseRacingTimingType.TimingType.None);
	    timingTween = DOTween.To(val =>
	                         {
		                         slider.value = val;
	                         }, 0.0f, 1.0f, roundTime)
	                         .SetEase(Ease.Linear)
	                         .SetLoops(-1, LoopType.Yoyo);
	    StartTimmingInternal().Forget();
    }

    private async UniTaskVoid StartTimmingInternal()
    {
	    var time = 3;
	    this.timer.SetEntity(time);
	    while (time >= 0)
	    {
		    await UniTask.Delay(TimeSpan.FromSeconds(1));
		    time--;
		    this.timer.SetEntity(time);
	    }
	    
	    if(result == UIHorseRacingTimingType.TimingType.None)
	    {
		    result = UIHorseRacingTimingType.TimingType.Bad;
		    timingType.SetEntity(result);
	    }
	    this.timer.SetEntity("GO");
	    await UniTask.Delay(TimeSpan.FromSeconds(1));
	    timingTween?.Kill(false);
	    timingTween = default;
	    this.entity.timingType.Invoke(result);
    }

    public void StopTimming()
    {
	    if (timingTween == default) return;
	    if (result != default) return;
	    
	    timingTween.Kill(false);
	    timingTween = default;
	    var markerIndex = markers.ToList()
	                             .FindLastIndex(x => IsInRange(slider.value, GetValueRange(x)));
	    result = (UIHorseRacingTimingType.TimingType)(markerIndex + 2);
	    timingType.SetEntity(result);
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