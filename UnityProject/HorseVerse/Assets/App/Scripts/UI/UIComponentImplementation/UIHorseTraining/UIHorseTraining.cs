using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseTraining : PopupEntity<UIHorseTraining.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentHorseDetail.Entity horseDetail;
        public UIComponentTraningState.Entity traningStates;
        public UIComponentTrainingPrepareState.Entity prepareState;
        public UIComponentTraningProcessingState.Entity processingState;
        public UIComponentTraningHorseSelectSumaryList.Entity horseSelectSumaryList;
    }

    public UIComponentHorseDetail horseDetail;
    public UIComponentTraningState traningStates;
    public UIComponentTrainingPrepareState prepareState;
    public UIComponentTraningProcessingState processingState;
    public UIComponentTraningHorseSelectSumaryList horseSelectSumaryList;

    protected override void OnSetEntity()
    {
        horseDetail.SetEntity(this.entity.horseDetail);
        traningStates.SetEntity(this.entity.traningStates);
        prepareState.SetEntity(this.entity.prepareState);
        processingState.SetEntity(this.entity.processingState);
        horseSelectSumaryList.SetEntity(this.entity.horseSelectSumaryList);
    }

    public void ChangeState(UIComponentTraningState.TraningState state)
    {
        this.traningStates.SetEntity(state);
    }
}	