using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeMyResult : UIComponent<UIComponentBetModeMyResult.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public bool isDoubleBet;
        public int horseNumberPrediction;
        public int horseNumberSecondPrediction;
        public float rate;
        public int spend;
        public int result;
    }

    public IsVisibleComponent bg;
    public FormattedTextComponent rate;
    public FormattedTextComponent spend;
    public UIComponentEnumInt horseNumberPrediction;
    public UIComponentEnumInt horseNumberSecondPrediction;
    public FormattedTextComponent result;

    protected override void OnSetEntity()
    {
        rate.SetEntity(this.entity.rate);
        spend.SetEntity(this.entity.spend);
        result.SetEntity(this.entity.result);
        horseNumberPrediction.SetEntity(this.entity.horseNumberPrediction);
        if (this.entity.isDoubleBet)
        {
            horseNumberSecondPrediction.gameObject.SetActive(true);
            horseNumberSecondPrediction.SetEntity(this.entity.horseNumberSecondPrediction);
        }
        else
            horseNumberSecondPrediction.gameObject.SetActive(false);
        bg.SetEntity(this.transform.GetSiblingIndex() % 2 == 1);
    }
}
