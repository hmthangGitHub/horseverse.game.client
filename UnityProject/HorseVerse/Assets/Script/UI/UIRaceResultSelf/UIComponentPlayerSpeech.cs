using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIComponentPlayerSpeech : UIComponent<UIComponentPlayerSpeech.Entity>
{
    [SerializeField]
    public class Entity
    {
        public string speech;
    }

    public FormattedTextComponent text;
    private Tween tween;

    protected override void OnSetEntity()
    {
    }

    public void OnEnable()
    {
        if (this.entity != null)
        {
            animateSpeech();
        }
    }

    private void animateSpeech()
    {
        tween?.Kill(true);
        tween = DOTween.To((val) =>
        {
            var len = (int)(this.entity.speech.Length * val);
            text.SetEntity(this.entity.speech.Substring(0, len));
        }, 0.0f, 1.0f, 1.0f).SetEase(Ease.Linear);
    }
}
