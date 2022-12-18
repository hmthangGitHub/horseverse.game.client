using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentEnumInt : UIComponent<UIComponentEnumInt.Entity>
{
    [Serializable]
    public class Entity
    {
        public int number;
    }

    public GameObject[] gameObjects;
    
    protected override void OnSetEntity()
    {
        gameObjects.ForEach((x, i) => x.SetActive(i == this.entity.number));
    }

    public void SetEntity(int number)
    {
        SetEntity(new Entity()
        {
            number = number
        });
    }
}
