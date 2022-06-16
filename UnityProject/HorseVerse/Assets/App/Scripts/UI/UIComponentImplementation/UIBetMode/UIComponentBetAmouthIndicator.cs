using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIComponentBetAmouthIndicator : UIComponent<UIComponentBetAmouthIndicator.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public Action<int> OnFocusIndex = _ => { };
        public UIComponentBetAmouthListScroller.Entity betAmouthListScroller;
    }
    public ButtonComponent nextBtn;
    public ButtonComponent previousBtn;
    public UIComponentBetAmouthListScroller betAmouthListScroller;

    public int currentFocusIndex = 0;
    public RectTransform viewportRectTransform;
    public RectTransform betAmouthListScrollerRectTransform;
    private Action<int> OnFocusIndex = ActionUtility.EmptyAction<int>.Instance;

    protected override void OnSetEntity()
    {
        nextBtn.SetEntity(OnNext);
        previousBtn.SetEntity(OnPrevious);
        betAmouthListScroller.SetEntity(this.entity.betAmouthListScroller);
        currentFocusIndex = 0;
        SetInitialListPosition();
        SetButtonsState();
        OnFocusIndex += this.entity.OnFocusIndex;
        OnFocusIndex.Invoke(currentFocusIndex);
    }

    private void OnPrevious()
    {
        currentFocusIndex--;
        OnFocusIndex.Invoke(currentFocusIndex);
        ChangeFocusIndexAnimation();
        SetButtonsState();
    }

    private void ChangeFocusIndexAnimation()
    {
        betAmouthListScrollerRectTransform.DOKill(true);
        var target = GetPosX(currentFocusIndex);
        betAmouthListScrollerRectTransform.DOAnchorPosX(target, 0.25f).SetEase(Ease.InExpo);
    }

    private void OnNext()
    {
        currentFocusIndex++;
        OnFocusIndex.Invoke(currentFocusIndex);
        ChangeFocusIndexAnimation();
        SetButtonsState();
    }

    private void SetButtonsState()
    {
        nextBtn.SetInteractable(currentFocusIndex < this.entity.betAmouthListScroller.entities.Length - 1);
        previousBtn.SetInteractable(currentFocusIndex > 0);
    }

    private void SetInitialListPosition()
    {
        var initalPosX = GetPosX(currentFocusIndex);
        betAmouthListScrollerRectTransform.anchoredPosition = new Vector2(initalPosX, 0);
    }

    private float GetPosX(int index)
    {
        return (viewportRectTransform.rect.width / 3) * - (index) + viewportRectTransform.rect.width / 3;
    }
}	