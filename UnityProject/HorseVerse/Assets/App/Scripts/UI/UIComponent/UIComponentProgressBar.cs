using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIComponentProgressBar : UIComponent<UIComponentProgressBar.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public float progress;
        public System.Action<float> OnChangeValue;
    }

    public Slider slider;
    private UnityEvent<float> progressEvent = new UnityEvent<float>();

    private void Awake()
    {
        this.slider.onValueChanged.AddListener((f) => progressEvent.Invoke(f));
    }

    protected override void OnSetEntity()
    {
        slider.value = this.entity.progress;
        progressEvent.RemoveAllListeners();
        progressEvent.AddListener((f) => this.entity.OnChangeValue?.Invoke(f));
    }

    public void SetEntity(float progress)
    {
        this.entity ??= new Entity()
        {
        };
        this.entity.progress = progress;
        OnSetEntity();
    }

    void Reset()
    {
        slider ??= this.GetComponent<Slider>();
    }
}	