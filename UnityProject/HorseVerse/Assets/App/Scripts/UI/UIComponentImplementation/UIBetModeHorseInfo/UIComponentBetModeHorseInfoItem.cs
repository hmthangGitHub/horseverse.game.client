using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeHorseInfoItem : UIComponent<UIComponentBetModeHorseInfoItem.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int no;
        public string horseName;
        public float rate;
        public float bestRec;
        public float avgRec;
        public float lastMatch;
        public ButtonSelectedComponent.Entity button;
    }

    public IsVisibleComponent bg;
    public UIComponentBetSlotNumber no;
    public FormattedTextComponent horseName;
    public FormattedTextComponent rate;
    public FormattedTextComponent bestRec;
    public FormattedTextComponent avgRec;
    public FormattedTextComponent lastMatch;
    public ButtonSelectedComponent button;

    protected override void OnSetEntity()
    {
        UIComponentBetSlotNumber.Entity e = no.entity;
        if (e == null)
        {
            e = new UIComponentBetSlotNumber.Entity();
        }
        e.Number = this.entity.no;
        no.SetEntity(e);
        horseName.SetEntity(this.entity.horseName);
        rate.SetEntity(this.entity.rate.ToString("0.00"));
        bestRec.SetEntity(this.entity.bestRec.ToString("0.00"));
        avgRec.SetEntity(this.entity.avgRec.ToString("0.00"));
        lastMatch.SetEntity(this.entity.lastMatch.ToString("0.00"));
        button.SetEntity(this.entity.button);
        bg.SetEntity(this.transform.GetSiblingIndex() % 2 == 1);
        this.gameObject.SetActive(true);
    }
}
