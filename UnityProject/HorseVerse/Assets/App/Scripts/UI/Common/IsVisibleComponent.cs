using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsVisibleComponent : UIComponent<IsVisibleComponent.Entity>
{
    [Serializable]
    public class Entity
    {
        public bool isVisible;
    }

    public new GameObject gameObject;

    protected override void OnSetEntity()
    {
        this.gameObject.SetActive(this.entity.isVisible);
    }

    public void SetEntity(bool isVisible)
    {
        this.entity = new Entity()
        {
            isVisible = isVisible
        };
        OnSetEntity();
    }
}
