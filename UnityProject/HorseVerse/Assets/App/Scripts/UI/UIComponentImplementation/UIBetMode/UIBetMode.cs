using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIBetMode : PopupEntity<UIBetMode.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentBetModeHeader.Entity header;
        public UIComponentSingleBetSlotList.Entity singleBetSlotList;
        public UIComponentDoubleBetList.Entity doubleBetSlotList;
        public UIComponentQuickBetButtonsContainer.Entity quickBetButtonsContainer;
        public UIComponentBetAmouthsContainer.Entity betAmouthsContainer;
    }

    public UIComponentBetModeHeader header;
    public UIComponentSingleBetSlotList singleBetSlotList;
    public UIComponentDoubleBetList doubleBetSlotList;
    public UIComponentQuickBetButtonsContainer quickBetButtonsContainer;
    public UIComponentBetAmouthsContainer betAmouthsContainer;
    public UIBetModeAnimation uiBetModeAnimation;
    
    protected override void OnSetEntity()
    {
        header.SetEntity(this.entity.header);
        singleBetSlotList.SetEntity(this.entity.singleBetSlotList);
        doubleBetSlotList.SetEntity(this.entity.doubleBetSlotList);
        quickBetButtonsContainer.SetEntity(this.entity.quickBetButtonsContainer);
        betAmouthsContainer.SetEntity(this.entity.betAmouthsContainer);
    }

    protected override UniTask AnimationIn()
    {
        return uiBetModeAnimation.AnimationIn();
    }

    protected override UniTask AnimationOut()
    {
        return uiBetModeAnimation.AnimationOut();
    }
}	