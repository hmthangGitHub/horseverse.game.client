using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.UI;
using Enum = System.Enum;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(ToggleGroup))]
public class UIComponentEnumToggleGroup<T> : UIComponent<UIComponentEnumToggleGroup<T>.Entity> where T : Enum
{
    [Serializable]
    public class Entity
    {
        public T initialValue;
        public Action<T> onToggleValueOn = ActionUtility.EmptyAction<T>.Instance;
    }

    [Serializable]
    public class TogglePair
    {
        public T enumValue;
        public UIComponentToggle toggle;
    }

    public TogglePair[] togglePairs;
    protected override void OnSetEntity()
    {
        togglePairs.ForEach(x =>
        {
            x.toggle.SetEntity(new UIComponentToggle.Entity()
            {
                isOn = this.entity.initialValue.Equals(x.enumValue),
                onActiveToggle = val =>
                {
                    if (val)
                    {
                        this.entity.onToggleValueOn.Invoke(x.enumValue);
                    }
                }
            });
        });
    }
#if UNITY_EDITOR
    private void Reset()
    {
        var toggleGroup = GetComponent<ToggleGroup>();
        
        togglePairs = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select((x, i) =>
            {
                var uiComponentToggle = transform.GetChild(i).GetComponent<UIComponentToggle>();
                uiComponentToggle.toggle.group = toggleGroup;
                return new TogglePair()
                {
                    toggle = uiComponentToggle,
                    enumValue = (T)(object)i
                };
            }).ToArray();
        
        EditorUtility.SetDirty(this);
    }
#endif
}
