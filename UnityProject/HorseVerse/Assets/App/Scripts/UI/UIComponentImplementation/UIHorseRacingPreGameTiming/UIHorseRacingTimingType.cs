using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIHorseRacingTimingType : UIComponentEnum<UIHorseRacingTimingType.TimingType>
{
    public enum TimingType
    {
        None,
        Bad,
        Good,
        Great,
        Perfect
    };

    protected override void OnSetEntity()
    {
        base.OnSetEntity();
        this.gameObjectList[(int)this.entity.enumEntity]
            .GetComponent<UISequenceAnimationBase>()
            .AnimationIn().Forget();
    }
}	