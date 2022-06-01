using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentEnum<TEnum> : UIComponent<UIComponentEnum<TEnum>.Entity> where TEnum : System.Enum
{
    [Serializable]
    public class Entity
    {
        public TEnum enumEntity;
    }

    public List<GameObject> gameObjectList = new List<GameObject>();

    protected override void OnSetEntity()
    {
        Array enumArray = Enum.GetValues(typeof(TEnum));
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].SetActive(EqualityComparer<TEnum>.Default.Equals((TEnum)enumArray.GetValue(i), this.entity.enumEntity));
        }
    }

    public void SetEntity(TEnum entity)
    {
        this.entity = new Entity()
        {
            enumEntity = entity
        };
        OnSetEntity();
    }
}
