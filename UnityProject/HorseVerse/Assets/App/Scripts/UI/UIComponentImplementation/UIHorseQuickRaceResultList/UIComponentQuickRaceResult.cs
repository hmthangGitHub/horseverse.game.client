using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentQuickRaceResult : UIComponent<UIComponentQuickRaceResult.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public int order;
        public string horseName;
        public UIComponentQuickRaceResultType.ResultType resultType;
    }

    public FormattedTextComponent order;
    public FormattedTextComponent horseName;
    public UIComponentQuickRaceResultType resultType;

    protected override void OnSetEntity()
    {
        order.SetEntity(this.entity.order);
        horseName.SetEntity(this.entity.horseName);
        resultType.SetEntity(this.entity.resultType);
    }
}	