using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetSlot : UIComponent<UIComponentBetSlot.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int horseNumber;
        public int firstHorseNumber;
        public int secondHorseNumber;
        public UIComponentBetSlotType.BetType betType;
        public float betRatio;
        public int totalBet;
        public ButtonComponent.Entity betBtn;
    }

    public FormattedTextComponent horseNumber;
    public FormattedTextComponent firstHorseNumber;
    public FormattedTextComponent secondHorseNumber;
    public UIComponentBetSlotType betType;
    public FormattedTextComponent betRatio;
    public FormattedTextComponent totalBet;
    public ButtonComponent betBtn;

    public CanvasGroup canvasGroup;

    protected override void OnSetEntity()
    {
        horseNumber.SetEntity(this.entity.horseNumber);
        firstHorseNumber.SetEntity(this.entity.firstHorseNumber);
        secondHorseNumber.SetEntity(this.entity.secondHorseNumber);
        betType.SetEntity(this.entity.betType);
        betRatio.SetEntity(this.entity.betRatio);
        totalBet.SetEntity(this.entity.totalBet);
        betBtn.SetEntity(this.entity.betBtn);
    }

    public void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1 : 0;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    public void SetTotalBetAmouth(int totalBet)
    {
        this.entity.totalBet = totalBet;
        this.totalBet.SetEntity(this.entity.totalBet);
    }
}	