using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FormattedTextComponent : UIComponent<FormattedTextComponent.Entity>
{
    [Serializable]
    public class Entity
    {
        public object[] param;
    }

    public TextMeshProUGUI text;
    public string format = "{0}";

    public void SetEntity(object value)
    {
        if (value is FormattedTextComponent.Entity entity)
        {
            this.entity = entity;
        }
        else
        {
            this.entity = new Entity()
            {
                param = new object[] { value }
            };
        }
        OnSetEntity();
    }

    public void SetEntity(params object[] param)
    {
        this.entity = new Entity()
        {
            param = param
        };
        OnSetEntity();
    }

    protected override void OnSetEntity()
    {
        text.text = string.Format(format, this.entity.param);
    }

    void Reset()
    {
        text ??= this.GetComponent<TextMeshProUGUI>();
    }
}
