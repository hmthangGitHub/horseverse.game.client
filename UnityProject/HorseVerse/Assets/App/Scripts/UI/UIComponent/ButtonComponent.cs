using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonComponent : UIComponent<ButtonComponent.Entity>
{

    [Serializable]
    public class Entity
    {
        public UnityEvent onClickEvent;
        public bool isInteractable = true;

        public Entity(Action action, bool isInteractable = true)
        {
            onClickEvent = new UnityEvent();
            onClickEvent.AddListener(() => action.Invoke());
            this.isInteractable = isInteractable;
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
    public string sfxName = "Click";
    public bool enableSFX = true;

    private UnityEvent buttonEvent = new UnityEvent();

    private void Awake()
    {
        this.button.onClick.AddListener(() => { buttonEvent.Invoke(); PlaySound(); });
    }

    protected override void OnSetEntity()
    {
        buttonEvent.RemoveAllListeners();
        buttonEvent.AddListener(() => this.entity.onClickEvent.Invoke());
        button.interactable = this.entity.isInteractable;
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

    public void SetInteractable(bool isInteractable)
    {
        if (this.entity != null)
        {
            this.entity.isInteractable = isInteractable;
            button.interactable = this.entity.isInteractable;
        }
    }

    void Reset()
    {
        button ??= this.GetComponent<Button>();
    }

    private void PlaySound()
    {
        if (!enableSFX) return;
        if (!string.IsNullOrEmpty(sfxName.Trim()))
            SoundController.PlaySFX(sfxName);
        else
            SoundController.PlayClick();
        Debug.Log("PLAY SOUND");
    }
}
