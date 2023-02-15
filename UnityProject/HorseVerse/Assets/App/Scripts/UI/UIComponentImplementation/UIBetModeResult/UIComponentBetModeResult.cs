using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentBetModeResult : UIComponent<UIComponentBetModeResult.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public int no;
        public string horseName;
        public int horseNumber;
        public float time;
        public UIComponentRaceRewardGroup.Entity rewardGroup;
        public bool rewardGroupVisible;
        public bool isSelfHorse;
    }

    public IsVisibleComponent bg;
    public FormattedTextComponent no;
    public FormattedTextComponent horseName;
    public UIComponentEnumInt horseNumber;
    public UIComponentTimeSpan time;
    public UIComponentRaceRewardGroup rewardGroup;
    public IsVisibleComponent rewardGroupVisible;
    public IsVisibleComponent isSelfHorse;

    protected override void OnSetEntity()
    {
        no.SetEntity(this.entity.no);
        horseName.SetEntity(this.entity.horseName);
        time.SetEntity(this.entity.time);
        horseNumber.SetEntity(this.entity.horseNumber);
        rewardGroup.SetEntity(this.entity.rewardGroup);
        rewardGroupVisible.SetEntity(this.entity.rewardGroupVisible);
        isSelfHorse.SetEntity(this.entity.isSelfHorse);
        bg.SetEntity(this.transform.GetSiblingIndex() % 2 == 1);
    }
}	