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
        public string secondHorseRankName;
        public UIComponentBetSlot.Entity betSlot;
        public bool betSlotVisible;
        public int selfRaceRank;
        public bool selfRaceRankGroup;
    }

    public HorseRaceStatusPlayerList playerList;
    public FormattedTextComponent singleRankHorseName;
    public FormattedTextComponent secondHorseRankName;
    public UIComponentTimeSpan timeText;
    public UIComponentBetSlot betSlot;
    public IsVisibleComponent isBetSlotVisible;
    public IsVisibleComponent selfRaceRankGroup;
    public FormattedTextComponent selfRaceRank;
    
    public Image timeLine;
    public float currentTimer = 0.0f;

    protected override void OnSetEntity()
    {
        currentTimer = 0.0f;
        playerList.SetEntity(this.entity.playerList);
        singleRankHorseName.SetEntity(this.entity.singleRankHorseName);
        secondHorseRankName.SetEntity(this.entity.secondHorseRankName);
        betSlot.SetEntity(this.entity.betSlot);
        isBetSlotVisible.SetEntity(this.entity.betSlotVisible);
        selfRaceRankGroup.SetEntity(this.entity.selfRaceRankGroup);
        UpdateSelfRank(this.entity.selfRaceRank);
    }

    private void Update()
    {
        if (this.entity != null)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer <= this.entity.finishTime)
            {
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

    public void UpdateFirstRank(string text)
    {
        if(this.entity != default)
        {
            this.entity.singleRankHorseName = text;
            singleRankHorseName.SetEntity(this.entity.singleRankHorseName);
        }
    }

    public void UpdateSecondRank(string text)
    {
        if (this.entity != default)
        {
            this.entity.secondHorseRankName = text;
            secondHorseRankName.SetEntity(this.entity.secondHorseRankName);
        }
    }
    
    public void UpdateSelfRank(int rank)
    {
        this.entity.selfRaceRank = rank + 1;
        selfRaceRank.SetEntity(this.entity.selfRaceRank, this.entity.selfRaceRank switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        });
    }
}
