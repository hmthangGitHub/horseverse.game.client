using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseRankResultList : UIComponent<UIComponentHorseRankResultList.Entity>
{
    [Serializable]
    public class Entity
    {
       //khong khai bao rankList duoc ;(, van chua giac ngo duoc ECS ;(
        public ButtonComponent.Entity closeBtn;
    }

    public ButtonComponent closeBtn;

    protected override void OnSetEntity()
    {
        closeBtn.SetEntity(this.entity.closeBtn);
    }
}
