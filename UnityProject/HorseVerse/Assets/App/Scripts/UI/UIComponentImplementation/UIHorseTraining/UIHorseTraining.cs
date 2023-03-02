using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIHorseTraining : PopupEntity<UIHorseTraining.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentHorseDetail.Entity horseDetail;
        public UIComponentHorseRace.Entity horseRace;
        public UIComponentTraningState.Entity traningStates;
        public UIComponentTrainingPrepareState.Entity prepareState;
        public UIComponentTraningProcessingState.Entity processingState;
        public UIComponentTraningHorseSelectSumaryList.Entity horseSelectSumaryList;
        public ButtonComponent.Entity leaderBoardBtn;
    }

    public UIComponentHorseDetail horseDetail;
    public UIComponentHorseRace horseRace;
    public UIComponentTraningState traningStates;
    public UIComponentTrainingPrepareState prepareState;
    public UIComponentTraningProcessingState processingState;
    public UIComponentTraningHorseSelectSumaryList horseSelectSumaryList;
    public ButtonComponent leaderBoardBtn;
    
    protected override void OnSetEntity()
    {
        horseDetail.SetEntity(this.entity.horseDetail);
        horseRace.SetEntity(this.entity.horseRace);
        traningStates.SetEntity(this.entity.traningStates);
        prepareState.SetEntity(this.entity.prepareState);
        processingState.SetEntity(this.entity.processingState);
        horseSelectSumaryList.SetEntity(this.entity.horseSelectSumaryList);
        leaderBoardBtn.SetEntity(this.entity.leaderBoardBtn);
    }

    public void ChangeState(UIComponentTraningState.TraningState state)
    {
        this.traningStates.SetEntity(state);
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