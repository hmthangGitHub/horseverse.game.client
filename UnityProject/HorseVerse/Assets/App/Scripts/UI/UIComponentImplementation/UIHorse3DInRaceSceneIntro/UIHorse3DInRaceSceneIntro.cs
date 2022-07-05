using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorse3DInRaceSceneIntro : PopupEntity<UIHorse3DInRaceSceneIntro.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIHorseModelLoader.Entity horseModelLoader;
    }

    public UIHorseModelLoader horseModelLoader;

    protected override void OnSetEntity()
    {
        horseModelLoader.SetEntity(this.entity.horseModelLoader);
    }
}	