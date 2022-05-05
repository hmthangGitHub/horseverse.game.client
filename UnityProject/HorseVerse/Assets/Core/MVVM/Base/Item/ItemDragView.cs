using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core.MVVM;
using Core.MVVM.UI;
using System;

namespace Core.MVVM.UI
{
    public class ItemDragView : ItemDragViewBase, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected Action<PointerEventData, bool> _callbackPointerEventData;

        protected virtual ItemDragView CloneItem()
        {
            return null;
        }

        private Vector3 _offset = Vector3.zero;
        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 newPosition = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, eventData.position, eventData.pressEventCamera, out newPosition);
            _offset = newPosition - transform.position;
            if (_callbackPointerEventData != null)
            {
                _callbackPointerEventData(eventData, false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 newPosition = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, eventData.position, eventData.pressEventCamera, out newPosition);
            transform.position = newPosition - _offset;
            if (_callbackPointerEventData != null)
            {
                _callbackPointerEventData(eventData, false);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector3 newPosition = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, eventData.position, eventData.pressEventCamera, out newPosition);
            if (_callbackPointerEventData != null)
            {
                _callbackPointerEventData(eventData, true);
            }
        }
    }

}