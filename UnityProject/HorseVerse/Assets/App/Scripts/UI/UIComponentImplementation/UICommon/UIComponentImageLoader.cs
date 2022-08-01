using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentImageLoader : UIComponent<UIComponentImageLoader.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public string image;
    }

    protected override void OnSetEntity()
    {
    }

    public void SetEntity(string image)
    {
        this.entity = new Entity()
        { 
            image = image 
        };
        OnSetEntity();
    }
}	