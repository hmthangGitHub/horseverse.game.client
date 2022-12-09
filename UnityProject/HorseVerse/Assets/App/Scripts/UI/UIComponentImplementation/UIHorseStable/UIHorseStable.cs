using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStable : PopupEntity<UIHorseStable.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentHorseStableAvatarList.Entity stableHorseAvatarList;
        public UIComponentHorseDetail.Entity horseDetail;
        public UIComponentHorseRace.Entity horseRace;
        public ButtonComponent.Entity breedingBtn;
        public ButtonComponent.Entity upgradeBtn;
    }

    public UIComponentHorseStableAvatarList stableHorseAvatarList;
    public UIComponentHorseDetail horseDetail;
    public UIComponentHorseRace horseRace;
    public ButtonComponent breedingBtn;
    public ButtonComponent upgradeBtn;

    protected override void OnSetEntity()
    {
        stableHorseAvatarList.SetEntity(this.entity.stableHorseAvatarList);
        horseDetail.SetEntity(this.entity.horseDetail);
        horseRace.SetEntity(this.entity.horseRace);
    }

    public void SetHorseDetailEntity(UIComponentHorseDetail.Entity entityHorseDetail)
    {
        entity.horseDetail = entityHorseDetail;
        horseDetail.SetEntity(entity.horseDetail);
        animation.PlayAnimationAsync(horseDetail.GetComponent<UIComponentHorseDetailAnimation>().CreateAnimation).Forget();
    }

    public void SetHorseRaceEntity(UIComponentHorseRace.Entity entityHorseRace)
    {
        entity.horseRace = entityHorseRace;
        horseRace.SetEntity(entity.horseRace);
    }
}	