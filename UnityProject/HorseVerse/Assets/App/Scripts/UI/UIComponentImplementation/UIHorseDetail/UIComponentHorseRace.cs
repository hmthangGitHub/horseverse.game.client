using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIComponentHorseRace : UIComponent<UIComponentHorseRace.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int type;
    }

    public GameObject[] images;

    protected override void OnSetEntity()
    {
        for(int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(i == entity.type);
        }
    }
}
