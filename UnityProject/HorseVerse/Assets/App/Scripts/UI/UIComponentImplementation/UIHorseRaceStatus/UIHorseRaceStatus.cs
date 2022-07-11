using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHorseRaceStatus : PopupEntity<UIHorseRaceStatus.Entity>
{
    [Serializable]
    public class Entity
    {
        public HorseRaceStatusPlayerList.Entity playerList;
        public float finishTime;
        public string singleRankHorseName;
        public string firstHorseRankName;
        public string secondHorseRankName;
        public UIComponentBetSlot.Entity betSlot;
        public bool betSlotVisible;
    }

    public HorseRaceStatusPlayerList playerList;
    public Image timeLine;
    public float currentTimer = 0.0f;
    public FormattedTextComponent singleRankHorseName;
    public FormattedTextComponent firstHorseRankName;
    public FormattedTextComponent secondHorseRankName;
    public UIComponentTimeSpan timeText;
    public UIComponentBetSlot betSlot;
    public IsVisibleComponent isBetSlotVisible;

    protected override void OnSetEntity()
    {
        currentTimer = 0.0f;
        playerList.SetEntity(this.entity.playerList);
        singleRankHorseName.SetEntity(this.entity.singleRankHorseName);
        firstHorseRankName.SetEntity(this.entity.firstHorseRankName);
        secondHorseRankName.SetEntity(this.entity.secondHorseRankName);
        betSlot.SetEntity(this.entity.betSlot);
        isBetSlotVisible.SetEntity(this.entity.betSlotVisible);
    }

    private void Update()
    {
        if (this.entity != null)
        {
            if (currentTimer <= this.entity.finishTime)
            {
                currentTimer += Time.deltaTime;
                timeLine.fillAmount = currentTimer / this.entity.finishTime;
                this.playerList.UpdatePosition(timeLine.fillAmount);
            }
            timeText.SetEntity(currentTimer);
        }
    }

    public void Skip()
    {
        this.currentTimer = this.entity.finishTime;
    }
}
