using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Core
{
    public class BasePopup : BaseElement
    {
        protected override void Awake()
        {

        }

        // Use this for initialization
        protected override void Start()
        {

        }

        // Update is called once per frame
        protected override void Update()
        {

        }

        protected sealed override void ShowDone()
        {
            base.ShowDone();
        }

        protected sealed override void HideDone()
        {
            LayerManager.Instance.PopupSystemCloseDone();
            LayerManager.Instance.HideLoading();
            LayerManager.Instance.HideDone(this);
            ReturnDefaultRectTransform();
            gameObject.SetActive(false);
            Dismiss();
        }

        protected override void Dismiss()
        {
            base.Dismiss();
        }

        public virtual void Close()
        {
            Hide();
        }

        protected sealed override void ShowTransitionNoneType()
        {
            ReturnDefaultRectTransform();
            ViewWillLoad();
            ShowDone();
        }

        protected sealed override void ShowTransitionLeftRightType()
        {
            RectTransform rect;
            Vector2 size;
            rect = GetComponent<RectTransform>();
            size = LayerManager.Instance.Size;
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(size.x, 0, 0);
            rect.sizeDelta = size;
            ViewWillLoad();

            Sequence backwardsTween;
            Vector3 finishPoint;
            backwardsTween = DOTween.Sequence();
            finishPoint = new Vector3(size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(ShowDone);
        }

        protected sealed override void ShowTransitionTopBottomType()
        {
            RectTransform rect;
            Vector2 size;
            rect = GetComponent<RectTransform>();
            size = LayerManager.Instance.Size;
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(size.x, 0, 0);
            rect.sizeDelta = size;
            ViewWillLoad();

            Sequence backwardsTween;
            Vector3 finishPoint;
            backwardsTween = DOTween.Sequence();
            finishPoint = new Vector3(size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(ShowDone);
        }

        protected sealed override void ShowTransitionFadeInOutType()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            ReturnDefaultRectTransform();
            ViewWillLoad();
            StartCoroutine(FadeIn(ShowDone, LayerManager.Instance.WaitingForFade));
        }

        protected sealed override void HideTransitionNoneType()
        {
            GetComponent<CanvasGroup>().alpha = 1;
            ReturnDefaultRectTransform();
            WillDismiss();
            HideDone();
        }

        protected sealed override void HideTransitionLeftRightType()
        {
            RectTransform rect = GetComponent<RectTransform>();
            Vector2 size = LayerManager.Instance.Size;
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(0, rect.position.y, 0);
            rect.sizeDelta = size;
            WillDismiss();

            Sequence backwardsTween;
            Vector3 finishPoint;
            backwardsTween = DOTween.Sequence();
            finishPoint = new Vector3(size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(HideDone);
        }

        protected sealed override void HideTransitionTopBottomType()
        {
            RectTransform rect = GetComponent<RectTransform>();
            Vector2 size = LayerManager.Instance.Size;
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(0, rect.position.y, 0);
            rect.sizeDelta = size;
            WillDismiss();

            Sequence backwardsTween;
            Vector3 finishPoint;
            backwardsTween = DOTween.Sequence();
            finishPoint = new Vector3(size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(HideDone);
        }

        protected sealed override void HideTransitionFadeInOutType()
        {
            GetComponent<CanvasGroup>().alpha = 1;
            WillDismiss();
            StartCoroutine(FadeOut(LayerManager.Instance.WaitingForFade));
        }
    }

}

