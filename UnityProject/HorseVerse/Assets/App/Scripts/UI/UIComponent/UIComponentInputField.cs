using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class UIComponentInputField : UIComponent<UIComponentInputField.Entity>
{
    [Serializable]
    public class Entity
    {
        public string placeHolderText;
        public string defaultValue;
        public Action<string> onValueChange = ActionUtility.EmptyAction<string>.Instance;
        public bool interactable = true;
    }
    public TMP_InputField inputField;
    private event Action<string> onValueChange = ActionUtility.EmptyAction<string>.Instance;

    private void Awake()
    {
        inputField.onValueChanged.AddListener(value => onValueChange.Invoke(value));
    }

    protected override void OnSetEntity()
    {
        onValueChange = ActionUtility.EmptyAction<string>.Instance;
        inputField.text = this.entity.defaultValue;
        if (!string.IsNullOrEmpty(entity.placeHolderText))
        {
            inputField.placeholder.GetComponent<FormattedTextComponent>().SetEntity(this.entity.placeHolderText);    
        }
        inputField.interactable = entity.interactable;
        onValueChange += this.entity.onValueChange;
    }
    
    void Reset()
    {
        inputField ??= this.GetComponent<TMP_InputField>();
        inputField.placeholder.gameObject.GetOrAddComponent<FormattedTextComponent>();
        inputField.textComponent.gameObject.GetOrAddComponent<FormattedTextComponent>();
    }
}
