using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeader : PopupEntity<UIHeader.Entity>
{
    [Serializable]
    public class Entity
    {
        public string userName;
        public int coin;
        public int maxEnergy;
        public int energy;
    }

    public FormattedTextComponent userName;
    public FormattedTextComponent coin;
    public FormattedTextComponent energy;

    protected override void OnSetEntity()
    {
        userName.SetEntity(this.entity.userName);
        coin.SetEntity(this.entity.coin);
        energy.SetEntity(this.entity.energy, this.entity.maxEnergy);
    }
}
