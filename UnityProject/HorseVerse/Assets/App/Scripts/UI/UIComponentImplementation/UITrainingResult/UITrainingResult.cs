using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITrainingResult : PopupEntity<UITrainingResult.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public long score;
        
        public ButtonComponent.Entity confirmBtn;
        public ButtonComponent.Entity retryBtn;
        public int currentEnergy;
        public int totalEnergy;
        public UITrainingResultRewardComponent.Entity boxReward;
        public UITrainingResultRewardComponent.Entity coinReward;
    }

    public TextMeshProUGUI score;
    public ButtonComponent confirmBtn;
    public ButtonComponent retryBtn;
    public FormattedTextComponent energy;
    public HorizontalLayoutGroup layout;
    public UITrainingResultRewardComponent boxReward;
    public UITrainingResultRewardComponent coinReward;

    protected override void OnSetEntity()
    {
        score.text = this.entity.score.ToString();
        confirmBtn.SetEntity(this.entity.confirmBtn);
        retryBtn.SetEntity(this.entity.retryBtn);
        energy.SetEntity(this.entity.currentEnergy.ToString(), this.entity.totalEnergy.ToString());
        boxReward.SetEntity(this.entity.boxReward);
        boxReward.gameObject.SetActive(this.entity.boxReward.Total > 0);
        coinReward.SetEntity(this.entity.coinReward);
        coinReward.gameObject.SetActive(this.entity.coinReward.Total > 0);
    }
}

