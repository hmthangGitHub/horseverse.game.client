using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetModeHorseInfo : PopupEntity<UIBetModeHorseInfo.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity backBtn;
        public UIComponentBetModeHorseInfoList.Entity horseList;
        public int horseDetailNumber;
        public UIComponentHorseDetail.Entity horseDetail;
        public UIComponentHorseRace.Entity horseRace;
    }

    public ButtonComponent backBtn;
    public UIComponentBetModeHorseInfoList horseList;
    [Header("Horse Detail Info")]
    public UIComponentHorseDetail horseDetail;
    public UIComponentHorseRace horseRace;
    public UIComponentBetSlotNumber horseDetailNumber;

    protected override void OnSetEntity()
    {
        backBtn.SetEntity(this.entity.backBtn);
        horseList.SetEntity(this.entity.horseList);
        horseDetail.SetEntity(this.entity.horseDetail);
        horseRace.SetEntity(this.entity.horseRace);
        UIComponentBetSlotNumber.Entity e = horseDetailNumber.entity;
        if (e == null)
        {
            e = new UIComponentBetSlotNumber.Entity();
        }
        e.Number = this.entity.horseDetailNumber;
        horseDetailNumber.SetEntity(e);
    }

    public void UpdateDetailInfo(Entity entity)
    {
        horseDetail.SetEntity(this.entity.horseDetail);
        horseRace.SetEntity(this.entity.horseRace);
        UIComponentBetSlotNumber.Entity e = horseDetailNumber.entity;
        if (e == null)
        {
            e = new UIComponentBetSlotNumber.Entity();
        }
        e.Number = this.entity.horseDetailNumber;
        horseDetailNumber.SetEntity(e);
    }

}
