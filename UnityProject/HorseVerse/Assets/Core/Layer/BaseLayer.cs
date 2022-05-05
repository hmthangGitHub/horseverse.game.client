using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class BaseLayer : BaseElement
    {
        private List<PopupState> _popupsStack = new List<PopupState>();
        protected override void Awake()
        {
            //		Debug.Log ("Awake: " + Time.time);
        }

        // Use this for initialization
        protected override void Start()
        {
            //		Debug.Log ("Start: " + Time.time);
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
        }

        public void Back()
        {
            if (_isClosing) return;
            LayerManager.Instance.Pop();
        }

        public virtual void ReceiveBackDone()
        {

        }

        public void BackPopup()
        {
            if (_popupsStack.Count > 1)
            {
                var currentPopup = GetCurrentPopup();
                currentPopup.Hide();
                _popupsStack.RemoveAt(_popupsStack.Count - 1);
                var currentState = _popupsStack[_popupsStack.Count - 1];
                currentPopup = currentState.Owner;
                currentPopup.transform.SetAsLastSibling();
            }
            else
            {
                var currentPopup = GetCurrentPopup();
                if(currentPopup != null)
                {
                    currentPopup.Hide();
                    _popupsStack.RemoveAt(_popupsStack.Count - 1);
                }
            }
        }

        protected override void Dismiss()
        {
            base.Dismiss();
        }

        public BasePopupInLayer GetCurrentPopup()
        {
            if(_popupsStack.Count > 0)
            {
                return _popupsStack[_popupsStack.Count - 1].Owner;
            }
            return null;
        }

        public BasePopupInLayer GetParentOfCurrentPopup()
        {
            if (_popupsStack.Count > 1)
            {
                return _popupsStack[_popupsStack.Count - 2].Owner;
            }
            return null;
        }

        public void PushPopup(BasePopupInLayer basePopupInLayer)
        {
            _popupsStack.Add(new PopupState(basePopupInLayer, this));
        }

        protected sealed override void ShowTransitionNoneType()
        {
			GetComponent<CanvasGroup>().alpha = 1;
			ReturnDefaultRectTransform();
            BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
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
            BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
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
			ViewWillLoad();
			RectTransform rect;
            Vector2 size = LayerManager.Instance.Size;
            Sequence backwardsTween;
            Vector3 finishPoint;
            BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
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
            BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
            if (parent != null)
            {
                parent.GetComponent<CanvasGroup>().alpha = 1;
				parent.WillDismiss();
				parent.StartCoroutine(parent.FadeOut(LayerManager.Instance.WaitingForFade));
				ViewWillLoad();
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
			BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
            if (parent != null)
            {
                HideDone();
                parent.gameObject.SetActive(true);
                parent.GetComponent<CanvasGroup>().alpha = 1;
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

            BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
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

            BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
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
			BaseLayer parent = LayerManager.Instance.GetParentOfCurrentLayer();
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
