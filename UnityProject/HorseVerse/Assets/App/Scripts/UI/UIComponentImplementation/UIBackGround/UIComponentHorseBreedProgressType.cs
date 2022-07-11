using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseBreedProgressType : UIComponent<UIComponentHorseBreedProgressType.Entity>
{
    
    public enum BreedType
    {
        Night,
        Thunder,
        Light
    }

	[System.Serializable]
    public class Entity
    {
        public float progress;
        public BreedType breedType;
    }

    public UIComponentImageProgress[] uIComponentImageProgresses;

    protected override void OnSetEntity()
    {
        for (int i = 0; i < uIComponentImageProgresses.Length; i++)
        {
            uIComponentImageProgresses[i].gameObject.SetActive(i == (int)this.entity.breedType);
        }
        uIComponentImageProgresses[(int)this.entity.breedType].SetEntity(new UIComponentImageProgress.Entity()
        {
            progress = this.entity.progress
        });
    }
}	