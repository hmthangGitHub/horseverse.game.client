using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Core
{
    public class BasePopupInLayer : BaseElement
    {
        public BaseLayer baseLayer { get; set; }

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
            LayerManager.Instance.HideLoading();
            LayerManager.Instance.HideDone(this);
            ReturnDefaultRectTransform();
            gameObject.SetActive(false);
            Dismiss();
            baseLayer = null;
        }

        public void Back()
        {
            if (_isClosing) return;
            baseLayer.BackPopup();
        }

        public virtual void ReceiveBackDone()
        {

        }

        protected override void Dismiss()
        {
            base.Dismiss();
        }

        //protected sealed override void ShowTransitionNoneType()
        //{
        //    ReturnDefaultRectTransform();
        //    ShowDone();
        //}

        //protected sealed override void ShowTransitionLeftRightType()
        //{
        //    RectTransform rect;
        //    Vector2 size;
        //    rect = GetComponent<RectTransform>();
        //    size = LayerManager.Instance.Size;
        //    rect.anchorMax = new Vector2(0, 0);
        //    rect.localPosition = new Vector3(size.x, 0, 0);
        //    rect.sizeDelta = size;

        //    Sequence backwardsTween;
        //    Vector3 finishPoint;
        //    backwardsTween = DOTween.Sequence();
        //    finishPoint = new Vector3(size.x / 2, size.y / 2);
        //    backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
        //    backwardsTween.OnComplete(ShowDone);
        //}

        //protected sealed override void ShowTransitionTopBottomType()
        //{
        //    RectTransform rect;
        //    Vector2 size;
        //    rect = GetComponent<RectTransform>();
        //    size = LayerManager.Instance.Size;
        //    rect.anchorMax = new Vector2(0, 0);
        //    rect.localPosition = new Vector3(size.x, 0, 0);
        //    rect.sizeDelta = size;

        //    Sequence backwardsTween;
        //    Vector3 finishPoint;
        //    backwardsTween = DOTween.Sequence();
        //    finishPoint = new Vector3(size.x / 2, size.y / 2);
        //    backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
        //    backwardsTween.OnComplete(ShowDone);
        //}

        //protected sealed override void ShowTransitionFadeInOutType()
        //{
        //    GetComponent<CanvasGroup>().alpha = 0;
        //    ReturnDefaultRectTransform();
        //    StartCoroutine(FadeIn(LayerManager.Instance.WaitingForFade));
        //}

        //protected sealed override void HideTransitionNoneType()
        //{
        //    GetComponent<CanvasGroup>().alpha = 1;
        //    ReturnDefaultRectTransform();
        //    HideDone();
        //}

        //protected sealed override void HideTransitionLeftRightType()
        //{
        //    RectTransform rect = GetComponent<RectTransform>();
        //    Vector2 size = LayerManager.Instance.Size;
        //    rect.anchorMax = new Vector2(0, 0);
        //    rect.localPosition = new Vector3(0, rect.position.y, 0);
        //    rect.sizeDelta = size;

        //    Sequence backwardsTween;
        //    Vector3 finishPoint;
        //    backwardsTween = DOTween.Sequence();
        //    finishPoint = new Vector3(size.x / 2, size.y / 2);
        //    backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
        //    backwardsTween.OnComplete(HideDone);
        //}

        //protected sealed override void HideTransitionTopBottomType()
        //{
        //    RectTransform rect = GetComponent<RectTransform>();
        //    Vector2 size = LayerManager.Instance.Size;
        //    rect.anchorMax = new Vector2(0, 0);
        //    rect.localPosition = new Vector3(0, rect.position.y, 0);
        //    rect.sizeDelta = size;

        //    Sequence backwardsTween;
        //    Vector3 finishPoint;
        //    backwardsTween = DOTween.Sequence();
        //    finishPoint = new Vector3(size.x / 2, size.y / 2);
        //    backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
        //    backwardsTween.OnComplete(HideDone);
        //}

        //protected sealed override void HideTransitionFadeInOutType()
        //{
        //    GetComponent<CanvasGroup>().alpha = 1;
        //    StartCoroutine(FadeOut(LayerManager.Instance.WaitingForFade));
        //}


        protected sealed override void ShowTransitionNoneType()
        {
			ReturnDefaultRectTransform();
            BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
				parent.WillDismiss();
				ViewWillLoad();
				parent.HideDone();
				ShowDone();
            }
            else
            {
				ViewWillLoad();
				ShowDone();
            }
        }

        protected sealed override void ShowTransitionLeftRightType()
        {
			RectTransform rect;
            Vector2 size = LayerManager.Instance.Size;
            Sequence backwardsTween;
            Vector3 finishPoint;
            BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
                parent.GetComponent<CanvasGroup>().alpha = 1;
                rect = GetComponent<RectTransform>();
                rect.anchorMax = new Vector2(0, 0);
                rect.localPosition = new Vector3(0, 0, 0);
                rect.sizeDelta = size;
				parent.WillDismiss();

				backwardsTween = DOTween.Sequence();
                finishPoint = new Vector3(-size.x / 2, size.y / 2);
                backwardsTween.Append(parent.transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
                backwardsTween.OnComplete(parent.HideDone);
            }

            GetComponent<CanvasGroup>().alpha = 1;
            rect = GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(size.x, 0, 0);
            rect.sizeDelta = size;
			ViewWillLoad();

			backwardsTween = DOTween.Sequence();

            finishPoint = new Vector3(size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(ShowDone);
        }

        protected sealed override void ShowTransitionTopBottomType()
        {
			RectTransform rect;
            Vector2 size = LayerManager.Instance.Size;
            Sequence backwardsTween;
            Vector3 finishPoint;
            BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
                parent.GetComponent<CanvasGroup>().alpha = 1;
                rect = GetComponent<RectTransform>();
                rect.anchorMax = new Vector2(0, 0);
                rect.localPosition = new Vector3(0, 0, 0);
                rect.sizeDelta = size;
				parent.WillDismiss();

				backwardsTween = DOTween.Sequence();
                finishPoint = new Vector3(-size.x / 2, size.y / 2);
                backwardsTween.Append(parent.transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
                backwardsTween.OnComplete(parent.HideDone);
            }

            GetComponent<CanvasGroup>().alpha = 1;
            rect = GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(size.x, 0, 0);
            rect.sizeDelta = size;
			ViewWillLoad();

			backwardsTween = DOTween.Sequence();

            finishPoint = new Vector3(size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(ShowDone);
        }

        protected sealed override void ShowTransitionFadeInOutType()
        {
			GetComponent<CanvasGroup>().alpha = 0;
            ReturnDefaultRectTransform();
            BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
                parent.GetComponent<CanvasGroup>().alpha = 1;
				parent.ReturnDefaultRectTransform();
				parent.WillDismiss();
				ViewWillLoad();
				parent.StartCoroutine(parent.FadeOut(LayerManager.Instance.WaitingForFade));
                StartCoroutine(FadeIn(ShowDone, LayerManager.Instance.WaitingForFade, LayerManager.Instance.WaitingForFade));
            }
            else
            {
				ViewWillLoad();
				StartCoroutine(FadeIn(ShowDone, LayerManager.Instance.WaitingForFade));
            }
        }

        protected sealed override void HideTransitionNoneType()
        {
            GetComponent<CanvasGroup>().alpha = 1;
            ReturnDefaultRectTransform();
			WillDismiss();
			BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
                HideDone();
                parent.gameObject.SetActive(true);
                parent.GetComponent<CanvasGroup>().alpha = 1;
				parent.ReturnDefaultRectTransform();
				parent.ViewWillLoad();
				parent.ShowDone();
                parent.ReceiveBackDone();
            }
            else
            {
                HideDone();
            }
        }

        protected sealed override void HideTransitionLeftRightType()
        {
            RectTransform rect = GetComponent<RectTransform>();
            GetComponent<CanvasGroup>().alpha = 1;
            Sequence backwardsTween;
            Vector3 finishPoint;
            Vector2 size = LayerManager.Instance.Size;
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(0, 0, 0);
            rect.sizeDelta = size;
			WillDismiss();

			backwardsTween = DOTween.Sequence();
            finishPoint = new Vector3(size.x + size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(HideDone);

            BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
                parent.gameObject.SetActive(true);
                parent.GetComponent<CanvasGroup>().alpha = 1;
                rect = parent.GetComponent<RectTransform>();
                rect.anchorMax = new Vector2(0, 0);
                rect.localPosition = new Vector3(-size.x, 0, 0);
                rect.sizeDelta = size;
				parent.ViewWillLoad();

				backwardsTween = DOTween.Sequence();
                finishPoint = new Vector3(size.x / 2, size.y / 2);
                backwardsTween.Append(parent.transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
                backwardsTween.OnComplete(() =>
                {
                    parent.ShowDone();
                    parent.ReceiveBackDone();
                });
            }
        }

        protected sealed override void HideTransitionTopBottomType()
        {
            RectTransform rect = GetComponent<RectTransform>();
            GetComponent<CanvasGroup>().alpha = 1;
            Sequence backwardsTween;
            Vector3 finishPoint;
            Vector2 size = LayerManager.Instance.Size;
            rect.anchorMax = new Vector2(0, 0);
            rect.localPosition = new Vector3(0, 0, 0);
            rect.sizeDelta = size;
			WillDismiss();

			backwardsTween = DOTween.Sequence();
            finishPoint = new Vector3(size.x + size.x / 2, size.y / 2);
            backwardsTween.Append(transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
            backwardsTween.OnComplete(HideDone);

            BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
                parent.gameObject.SetActive(true);
                parent.GetComponent<CanvasGroup>().alpha = 1;
                rect = parent.GetComponent<RectTransform>();
                rect.anchorMax = new Vector2(0, 0);
                rect.localPosition = new Vector3(-size.x, 0, 0);
                rect.sizeDelta = size;
				parent.ViewWillLoad();

				backwardsTween = DOTween.Sequence();
                finishPoint = new Vector3(size.x / 2, size.y / 2);
                backwardsTween.Append(parent.transform.DOMove(finishPoint, LayerManager.Instance.WaitingForTranlate).SetEase(Ease.Linear));
                backwardsTween.OnComplete(() =>
                {
                    parent.ShowDone();
                    parent.ReceiveBackDone();
                });
            }
        }

        protected sealed override void HideTransitionFadeInOutType()
        {
            GetComponent<CanvasGroup>().alpha = 1;
			ReturnDefaultRectTransform();
			WillDismiss();
			BasePopupInLayer parent = baseLayer.GetParentOfCurrentPopup();
            if (parent != null)
            {
                StartCoroutine(FadeOut(LayerManager.Instance.WaitingForFade));
                parent.gameObject.SetActive(true);
                parent.GetComponent<CanvasGroup>().alpha = 0;
				parent.ReturnDefaultRectTransform();
				parent.ViewWillLoad();
				parent.StartCoroutine(parent.FadeIn(() =>
                {
					parent.ShowDone();
					parent.ReceiveBackDone();
                }, LayerManager.Instance.WaitingForFade, LayerManager.Instance.WaitingForFade));
            }
            else
            {
                StartCoroutine(FadeOut(LayerManager.Instance.WaitingForFade));
            }
        }
    }

}

