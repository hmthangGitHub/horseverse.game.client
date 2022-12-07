using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonSelectedComponent : UIComponent<ButtonSelectedComponent.Entity>
{
    [Serializable]
    public class Entity
    {
        public UnityEvent onClickEvent;
        public bool isInteractable = true;
        public bool isSelected = false;

        public Entity(Action action, bool isSelected,  bool isInteractable = true)
        {
            onClickEvent = new UnityEvent();
            onClickEvent.AddListener(() => action.Invoke());
            this.isInteractable = isInteractable;
            this.isSelected = isSelected;
        }

        public Entity(UnityEvent buttonEvent, bool isSelected = false)
        {
            this.onClickEvent = buttonEvent;
            this.isSelected = isSelected;
        }

        public Entity()
        {
        }
    }

    public Button button;
    public Animator anim;
    public bool IsSelected;

    private UnityEvent buttonEvent = new UnityEvent();

    private void Awake()
    {
        this.button.onClick.AddListener(() => buttonEvent.Invoke()); 
    }

    protected override void OnSetEntity()
    {
        buttonEvent.RemoveAllListeners();
        buttonEvent.AddListener(() => this.entity.onClickEvent.Invoke());
        button.interactable = this.entity.isInteractable;
        anim.SetBool("IsSelected", this.entity.isSelected);
        anim.SetTrigger("Normal");
        delayToSetAnim().Forget();
    }

    public void SetEntity(UnityEvent buttonEvent)
    {
        this.entity = new Entity(buttonEvent);
        OnSetEntity();
    }

    public void SetEntity(Action action, bool isSelected = false)
    {
        this.entity = new Entity(action, isSelected);
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

    public void SetSelected(bool isSelected)
    {
        if (this.entity != null)
        {
            this.entity.isSelected = isSelected;
            anim.SetBool("IsSelected", this.entity.isSelected);
            anim.SetTrigger("Normal");
        }
    }

    protected async UniTask delayToSetAnim()
    {
        await UniTask.Delay(200);
        anim.SetBool("IsSelected", this.entity.isSelected);
        anim.SetTrigger("Normal");
    }

    void Reset()
    {
        button ??= this.GetComponent<Button>();
        anim ??= this.GetComponent<Animator>();
    }
}
