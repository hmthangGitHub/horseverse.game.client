using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VerticalInputComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public event Action<Vector3> OnUpdateDirection;
    public event Action<bool> OnHolding;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform background;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] bool autoHide;
    [Range(0.25f, 1f)] public float clampRate = 1f;
    [Tooltip("If the handle is inside this range, in proportion to the background size, the output is zero.")]
    [Range(0f, 0.5f)]
    public float deadZone = 0.2f;

    public bool IsWorking { get; private set; } = false;
    public Vector2 Output { get; private set; } = Vector2.zero;

    void Update()
    {
        OnUpdateDirection?.Invoke(Output * (2 - clampRate));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvasGroup != null && autoHide)
        {
            canvasGroup.DOFade(0, 1, 0.1f);
        }
        OnBackgroundDrag(eventData);
        OnHolding?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnBackgroundPointerUp(eventData);
        if (canvasGroup != null && autoHide)
        {
            canvasGroup.DOFade(1.0f, 0, 0.1f);
        }
        
        OnHolding?.Invoke(false);
    }

    public void OnBackgroundPointerUp(BaseEventData baseEventData)
    {
        IsWorking = false;
        Output = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsWorking || RectTransformUtility.RectangleContainsScreenPoint(background, eventData.position, eventData.pressEventCamera))
            OnBackgroundDrag(eventData);
    }

    public void OnBackgroundDrag(BaseEventData baseEventData)
    {
        PointerEventData eventData = baseEventData as PointerEventData;
        Vector2 point = eventData.position;
        Vector2 press = eventData.pressPosition;

        Vector2 vector = point - (Vector2)background.position;
        vector.y = 0;

        float magnitude = vector.magnitude;

        float deadSize = deadZone * background.sizeDelta.x * 0.5f;

        if (magnitude < deadSize)
        {
            Output = Vector2.zero;
            IsWorking = false;
            return;
        }

        Output = vector.normalized;

        IsWorking = true;
    }


}
