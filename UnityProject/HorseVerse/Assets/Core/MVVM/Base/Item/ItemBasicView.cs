using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core.MVVM;
using Core.MVVM.UI;
using UniRx;
using System;
using UniRx;

namespace Core.MVVM.UI
{
    public class ItemBasicView : ItemBasicViewBase
    {
        [SerializeField]
        protected RectTransform UI;
        public Button ClickButton;
        ItemBasicView _itemBasicViewClone = null;
        IDisposable _selectDisposable = null;
        IDisposable _unselectDisposable = null;

        protected override void AfterBind()
        {
            base.AfterBind();

            if (ClickButton != null)
            {
                ClickButton.onClick.AddListener(() =>
                {
                    ItemBasic.OnClickCommand.Execute();
                });
            }
        }

        protected override void SetDragLogicForItemExecuted(SetDragLogicForItemCommand command)
        {
            UI = command.UI;
            if (ClickButton != null)
            {
                ClickButton.enabled = !command.IsDrag;
            }
            if (_selectDisposable != null)
            {
                _selectDisposable.Dispose();
                _selectDisposable = null;
            }
            if (_unselectDisposable != null)
            {
                _unselectDisposable.Dispose();
                _unselectDisposable = null;
            }
            var eventTrigger = gameObject.GetComponent<EventTrigger>();
            if (command.IsDrag)
            {
                if (eventTrigger != null)
                {
                    Destroy(eventTrigger);
                }
                eventTrigger = gameObject.AddComponent<EventTrigger>();
                //if(eventTrigger == null){
                //    eventTrigger = gameObject.AddComponent<EventTrigger>();
                //}
                _selectDisposable = ItemBasic.SelectCommand.Subscribe(_ =>
                {
                    eventTrigger.enabled = false;
                });
                _unselectDisposable = ItemBasic.UnSelectCommand.Subscribe(_ =>
                {
                    eventTrigger.enabled = true;
                });
                //var pointerDown = new EventTrigger.Entry();
                //pointerDown.eventID = EventTriggerType.PointerDown;
                //IDisposable disposableWaitingDrag = null;
                //pointerDown.callback.AddListener((e) => {
                //    Debug.Log("Point down: " + Time.deltaTime);
                //    PointerEventData pointerEventData = e as PointerEventData;
                //    disposableWaitingDrag = Observable.Timer(TimeSpan.FromSeconds(command.TimeToCheckDrag)).Subscribe(_ =>
                //    {
                //        Debug.Log("can drag: " + Time.deltaTime);

                //    });
                //});
                //eventTrigger.triggers.Add(pointerDown);

                //var pointerUp = new EventTrigger.Entry();
                //pointerUp.eventID = EventTriggerType.PointerUp;
                //pointerUp.callback.AddListener((e) => {
                //    Debug.Log("Point up: " + Time.deltaTime);
                //    PointerEventData pointerEventData = e as PointerEventData;
                //    eventTrigger.enabled = true;
                //    if(disposableWaitingDrag != null){
                //        disposableWaitingDrag.Dispose();
                //        disposableWaitingDrag = null;
                //    }
                //});
                //eventTrigger.triggers.Add(pointerUp);

                var beginDrag = new EventTrigger.Entry();
                beginDrag.eventID = EventTriggerType.BeginDrag;
                beginDrag.callback.AddListener((e) =>
                {
                    PointerEventData pointerEventData = e as PointerEventData;
                    Vector3 newPosition = Vector3.zero;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, pointerEventData.position, pointerEventData.pressEventCamera, out newPosition);
                    if (command.HasClone)
                    {
                        var itemBasicViewModel = MVVM.CreateViewModel(ItemBasic.GetType());
                        _itemBasicViewClone = InstantiateView<ItemBasicView>(itemBasicViewModel, gameObject, UI.gameObject);
                        _itemBasicViewClone.transform.position = newPosition;
                        _itemBasicViewClone.transform.localScale = Vector3.one;
                        var rectTransform = _itemBasicViewClone.GetComponent<RectTransform>();
                        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        _itemBasicViewClone.GetComponent<RectTransform>().sizeDelta = command.Size;
                    }
                    else
                    {
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, pointerEventData.position, pointerEventData.pressEventCamera, out newPosition);
                        transform.position = newPosition;
                    }
                });
                eventTrigger.triggers.Add(beginDrag);

                var drag = new EventTrigger.Entry();
                drag.eventID = EventTriggerType.Drag;
                drag.callback.AddListener((e) =>
                {
                    PointerEventData pointerEventData = e as PointerEventData;
                    Vector3 newPosition = Vector3.zero;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, pointerEventData.position, pointerEventData.pressEventCamera, out newPosition);
                    if (command.HasClone)
                    {
                        _itemBasicViewClone.transform.position = newPosition;
                    }
                    else
                    {
                        transform.position = newPosition;
                    }
                });
                eventTrigger.triggers.Add(drag);

                var endDrag = new EventTrigger.Entry();
                endDrag.eventID = EventTriggerType.EndDrag;
                endDrag.callback.AddListener((e) =>
                {
                    PointerEventData pointerEventData = e as PointerEventData;
                    RectTransform rectTransformSelected = null;

                    Vector2 localpoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(UI, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);

                    for (int i = 0; i < command.List.Count; i++)
                    {
                        var rectTransform = command.List[i];
                        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera))
                        {
                            rectTransformSelected = rectTransform;
                            break;
                        }
                    }
                    if (rectTransformSelected != null)
                    {
                        if (command.Callback != null)
                        {
                            command.Callback(rectTransformSelected, ItemBasic.Data);
                        }
                    }
                    else
                    {
                        if (command.CallbackNoContainInList != null)
                        {
                            command.CallbackNoContainInList();
                        }
                    }
                    if (command.HasClone)
                    {
                        _itemBasicViewClone.Destroy();
                    }
                });
                eventTrigger.triggers.Add(endDrag);
            }
            else
            {
                if (eventTrigger != null)
                {
                    Destroy(eventTrigger);
                }
            }
        }
    }
}