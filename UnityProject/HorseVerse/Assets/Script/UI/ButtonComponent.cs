using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonComponent : LeaderBoardUIComponent<ButtonComponent.Entity>
{

    [Serializable]
    public class Entity
    {
        public UnityEvent onClickEvent;

        public Entity(Action action)
        {
            onClickEvent = new UnityEvent();
            onClickEvent.AddListener(() => action.Invoke());
        }

        public Entity(UnityEvent buttonEvent)
        {
            this.onClickEvent = buttonEvent;
        }

        public Entity()
        {
        }
    }

    public Button button;

    private UnityEvent buttonEvent = new UnityEvent();

    private void Awake()
    {
        this.button.onClick.AddListener(() => buttonEvent.Invoke());
    }

    protected override void OnSetEntity()
    {
        buttonEvent.RemoveAllListeners();
        buttonEvent.AddListener(() => this.entity.onClickEvent.Invoke());
    }

    public void SetEntity(UnityEvent buttonEvent)
    {
        this.entity = new Entity(buttonEvent);
        OnSetEntity();
    }

    public void SetEntity(Action action)
    {
        this.entity = new Entity(action);
        OnSetEntity();
    }

    void Reset()
    {
        button ??= this.GetComponent<Button>();
    }
}
