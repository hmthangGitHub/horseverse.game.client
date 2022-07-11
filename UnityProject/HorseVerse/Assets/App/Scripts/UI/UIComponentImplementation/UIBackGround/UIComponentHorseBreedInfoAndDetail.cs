using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseBreedInfoAndDetail : UIComponent<UIComponentHorseBreedInfoAndDetail.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentHorseBreedProgressList.Entity horseBreedProgressList;
        public UIComponentHorseDetail.Entity horseDetail;
    }

    public UIComponentHorseBreedProgressList horseBreedProgressList;
    public UIComponentHorseDetail horseDetail;

    protected override void OnSetEntity()
    {
        horseBreedProgressList.SetEntity(this.entity.horseBreedProgressList);
        horseDetail.SetEntity(this.entity.horseDetail);
    }
}	