using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIQuickMode : PopupEntity<UIQuickMode.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public ButtonComponent.Entity backBtn;
        public ButtonComponent.Entity findMatchBtn;
        public ButtonComponent.Entity cancelMatchBtn;
        public IsVisibleComponent.Entity findMatchBtnVisible;
        public IsVisibleComponent.Entity cancelMatchBtnVisible;
        public int findMatchEnergyCost;
        public UIComponentDuration.Entity findMatchTimer;
        public UIComponentHorseDetail.Entity horseDetail;
        public UIComponentTraningHorseSelectSumaryList.Entity horseSelectSumaryList;
        public UIComponentRaceRoomInfo.Entity raceRoomInfo;
    }

    public ButtonComponent backBtn;
    public FormattedTextComponent findMatchEnergyCost;
    public ButtonComponent findMatchBtn;
    public ButtonComponent cancelMatchBtn;
    public IsVisibleComponent findMatchBtnVisible;
    public IsVisibleComponent cancelMatchBtnVisible;
    public UIComponentDuration findMatchTimer;
    public UIComponentHorseDetail horseDetail;
    public UIComponentTraningHorseSelectSumaryList horseSelectSumaryList;
    public UIComponentRaceRoomInfo raceRoomInfo;

    protected override void OnSetEntity()
    {
        backBtn.SetEntity(this.entity.backBtn);
        findMatchEnergyCost.SetEntity(this.entity.findMatchEnergyCost);
        findMatchBtn.SetEntity(this.entity.findMatchBtn);
        cancelMatchBtn.SetEntity(this.entity.cancelMatchBtn);
        findMatchTimer.SetEntity(this.entity.findMatchTimer);
        horseDetail.SetEntity(this.entity.horseDetail);
        horseSelectSumaryList.SetEntity(this.entity.horseSelectSumaryList);
        findMatchBtnVisible.SetEntity(this.entity.findMatchBtnVisible);
        cancelMatchBtnVisible.SetEntity(this.entity.cancelMatchBtnVisible);
        raceRoomInfo.SetEntity(this.entity.raceRoomInfo);
    }

    public void SetHorseDetailEntity(UIComponentHorseDetail.Entity entityHorseDetail)
    {
        entity.horseDetail = entityHorseDetail;
        horseDetail.SetEntity(entity.horseDetail);
        animation.PlayAnimationAsync(horseDetail.GetComponent<UIComponentHorseDetailAnimation>().CreateAnimation).Forget();
    }
}	