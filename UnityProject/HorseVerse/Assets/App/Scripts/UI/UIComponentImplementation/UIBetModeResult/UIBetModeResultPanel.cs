using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class UIBetModeResultPanel : UIComponent<UIBetModeResultPanel.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentBetModeResult.Entity[] betModeResultList;
    }

    public UIComponentBetModeResultList betModeResultList;

    protected override void OnSetEntity()
    {
        betModeResultList.SetEntity(this.entity.betModeResultList);
    }
}
