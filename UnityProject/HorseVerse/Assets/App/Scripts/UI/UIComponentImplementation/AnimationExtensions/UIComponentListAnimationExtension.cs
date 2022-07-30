using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class UIComponentListAnimationExtension
{
    public static Sequence CreateSequenceAnimation<Template, TemplateEntity>(this UIComponentList<Template, TemplateEntity> uiComponentList, Func<Template, Tween> tweenFunction, bool isSequence, float delay) where TemplateEntity : new()
                                                                                                where Template : UIComponent<TemplateEntity>
    {
        return uiComponentList.instanceList.Select(tweenFunction).ToArray().AsSequence(isSequence, delay);
    }
}
