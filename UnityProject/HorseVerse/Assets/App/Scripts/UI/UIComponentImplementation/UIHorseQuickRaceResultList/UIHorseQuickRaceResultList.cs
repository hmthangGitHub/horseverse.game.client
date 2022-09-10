using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIHorseQuickRaceResultList : PopupEntity<UIHorseQuickRaceResultList.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public string[] horseNames;
        public ButtonComponent.Entity outerBtn;
    }

    public UIComponentQuickRaceResultList quickRaceResultList;
    public ButtonComponent outerBtn;

    protected override void OnSetEntity()
    {
        outerBtn.SetEntity(this.entity.outerBtn);
        quickRaceResultList.SetEntity(new UIComponentQuickRaceResultList.Entity()
        {
            entities = this.entity.horseNames.Select((x, i) => new UIComponentQuickRaceResult.Entity() 
            {
                horseName = x,
                order = i + 1,
                resultType = GetResultTypeFromIndex(i)
            }).ToArray()
        });
    }

    private UIComponentQuickRaceResultType.ResultType GetResultTypeFromIndex(int i)
    {
        return i switch
        {
            0 => UIComponentQuickRaceResultType.ResultType.First,
            1 => UIComponentQuickRaceResultType.ResultType.Second,
            _ => UIComponentQuickRaceResultType.ResultType.Other
        };
    }
}	