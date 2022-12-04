using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITrainingResultRewardComponent : UIComponent<UITrainingResultRewardComponent.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int Total;
    }

    public TextMeshProUGUI Total;

    protected override void OnSetEntity()
    {
        Total.text = this.entity.Total.ToString();
    }
}
