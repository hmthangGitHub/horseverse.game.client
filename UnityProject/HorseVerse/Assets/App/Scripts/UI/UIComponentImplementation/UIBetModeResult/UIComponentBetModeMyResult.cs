using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeMyResult : UIComponent<UIComponentBetModeMyResult.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public bool isDoubleBet;
        public int horseNumberFirst;
        public int horseNumberSecond;
        public float rate;
        public int spend;
        public int result;
    }

    public IsVisibleComponent bg;
    public FormattedTextComponent rate;
    public FormattedTextComponent spend;
    public UIComponentBetSlotNumber horseNumberFirst;
    public UIComponentBetSlotNumber horseNumberSecond;
    public FormattedTextComponent result;

    protected override void OnSetEntity()
    {
        rate.SetEntity(this.entity.rate);
        spend.SetEntity(this.entity.spend);
        result.SetEntity(this.entity.result);
        horseNumberFirst.SetEntity(this.entity.horseNumberFirst);
        if (this.entity.isDoubleBet)
        {
            horseNumberSecond.gameObject.SetActive(true);
            horseNumberSecond.SetEntity(this.entity.horseNumberSecond);
        }
        else
            horseNumberSecond.gameObject.SetActive(false);
        bg.SetEntity(this.transform.GetSiblingIndex() % 2 == 1);
    }
}
