using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIElementAnimationBase : UIAnimationBase
{
    public Transform animationTransform;
    public Button button;
    public Toggle toggle;
    private void Awake()
    {
        if (animationTransform == default)
        {
            animationTransform = transform;
        }
        button?.onClick.AddListener(OnClick);
        toggle?.onValueChanged.AddListener(val =>
        {
            if (val) OnClick();
        });
    }

    private void OnClick()
    {
        PlayAnimationAsync(OnClickAnimation).Forget();
    }

    protected abstract Tween OnClickAnimation();
    
    private void Reset()
    {
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();
    }
}