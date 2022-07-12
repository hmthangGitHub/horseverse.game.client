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
        public float time;
    }

    public IsVisibleComponent bg;
    public FormattedTextComponent no;
    public FormattedTextComponent horseName;
    public UIComponentTimeSpan time;

    protected override void OnSetEntity()
    {
        no.SetEntity(this.entity.no);
        horseName.SetEntity(this.entity.horseName);
        time.SetEntity(this.entity.time);
        bg.SetEntity(this.transform.GetSiblingIndex() % 2 == 1);
    }
}	