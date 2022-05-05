using UnityEngine;
using System.Collections;
using System;

namespace Core
{
    public class ParamaterBase
    {
    }

    public struct ElementParameter
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public ParamaterBase Parameter { get; private set; }
        public TransitionType TransitionType { get; private set; }

        public ElementParameter(string path, TransitionType type = TransitionType.None)
        {
            Path = path;
            Parameter = null;
            TransitionType = type;
            var data = Path.Split(new char[1] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
            Name = data[data.Length - 1];
        }

        public ElementParameter(string path, ParamaterBase parameter, TransitionType type = TransitionType.None)
        {
            Path = path;
            Parameter = parameter;
            TransitionType = type;
            var data = Path.Split(new char[1] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
            Name = data[data.Length - 1];
        }
    }

    public enum TransitionType
    {
        None,
        TransitionLeftRightType,
        TransitionTopBottomType,
        TransitionFadeInOutType,
    }

    public abstract class BaseElement : MonoBehaviour
    {
        private TransitionType _transition = TransitionType.TransitionLeftRightType;
        public TransitionType Transition
        {
            get
            {
                return _transition;
            }
            set
            {
                _transition = value;
            }
        }

        public ElementParameter LayerParameter { get; set; }

        protected bool _isClosing = false;


        protected virtual void Awake()
        {

        }

        // Use this for initialization
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        protected virtual void ReturnDefaultRectTransform()
        {
            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(1, 1);
            rect.localPosition = new Vector3(0, 0, 0);
            rect.sizeDelta = new Vector2(0, 0);
        }

        public void Show()
        {
            _isClosing = false;
            LayerManager.Instance.ShowLoadingHaveNoIcon();
            switch (_transition)
            {
                case TransitionType.None:
                    {
                        ShowTransitionNoneType();
                    }
                    break;
                case TransitionType.TransitionLeftRightType:
                    {
                        ShowTransitionLeftRightType();
                    }
                    break;
                case TransitionType.TransitionTopBottomType:
                    {
                        ShowTransitionTopBottomType();
                    }
                    break;
                case TransitionType.TransitionFadeInOutType:
                    {
                        ShowTransitionFadeInOutType();
                    }
                    break;
            }
        }

        public void Hide()
        {
            if (_isClosing) return;
            _isClosing = true;
            LayerManager.Instance.ShowLoadingHaveNoIcon();
            switch (_transition)
            {
                case TransitionType.None:
                    {
                        HideTransitionNoneType();
                    }
                    break;
                case TransitionType.TransitionLeftRightType:
                    {
                        HideTransitionLeftRightType();
                    }
                    break;
                case TransitionType.TransitionTopBottomType:
                    {
                        HideTransitionTopBottomType();
                    }
                    break;
                case TransitionType.TransitionFadeInOutType:
                    {
                        HideTransitionFadeInOutType();
                    }
                    break;
            }
        }

        protected IEnumerator FadeIn(Action onComplete, float maxTime, float delayTime = 0)
        {
            yield return new WaitForEndOfFrame();
            float currentDelayTime = 0;
            while (true)
            {
                if (currentDelayTime >= delayTime)
                {
                    break;
                }
                currentDelayTime += Time.deltaTime;
                yield return new WaitForSeconds(0);
            }

            float currentTime = 0;
            while (true)
            {
                currentTime += Time.deltaTime;
                GetComponent<CanvasGroup>().alpha = currentTime / maxTime;
                if (currentTime >= maxTime)
                {
                    GetComponent<CanvasGroup>().alpha = 1;
                    break;
                }
                yield return new WaitForSeconds(0);
            }
            onComplete();
        }

        protected virtual void ShowDone()
        {
            ViewDidLoad();
            ReturnDefaultRectTransform();
            LayerManager.Instance.HideLoading();
        }

        public IEnumerator FadeOut(float maxTime)
        {
            yield return new WaitForEndOfFrame();
            float currentTime = 0;
            while (true)
            {
                currentTime += Time.deltaTime;
                GetComponent<CanvasGroup>().alpha = 1 - currentTime / maxTime;
                if (currentTime >= maxTime)
                {
                    GetComponent<CanvasGroup>().alpha = 0;
                    break;
                }
                yield return new WaitForSeconds(0);
            }
            HideDone();
        }

        protected virtual void HideDone()
        {
            ReturnDefaultRectTransform();
            Dismiss();
            gameObject.SetActive(false);
        }

        protected abstract void ShowTransitionNoneType();

        protected abstract void ShowTransitionLeftRightType();

        protected abstract void ShowTransitionTopBottomType();

        protected abstract void ShowTransitionFadeInOutType();

        protected abstract void HideTransitionNoneType();

        protected abstract void HideTransitionLeftRightType();

        protected abstract void HideTransitionTopBottomType();

        protected abstract void HideTransitionFadeInOutType();

        protected virtual void WillDismiss()
        {
        }

        protected virtual void Dismiss()
        {

        }

        protected virtual void ViewWillLoad()
        {
        }

        protected virtual void ViewDidLoad()
        {

        }
    }

}