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
    }

    public HorseRaceStatusPlayerList playerList;
    public Image timeLine;
    public float currentTimer = 0.0f;

    protected override void OnSetEntity()
    {
        currentTimer = 0.0f;
        playerList.SetEntity(this.entity.playerList);
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
        }
    }

    public void Skip()
    {
        this.currentTimer = this.entity.finishTime;
    }
}
