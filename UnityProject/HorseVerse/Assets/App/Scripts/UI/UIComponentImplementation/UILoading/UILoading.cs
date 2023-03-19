using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoading : PopupEntity<UILoading.Entity>
{
    [Serializable]
    public class Entity
    {
        public bool loadingHorse = true;
    }

    public IsVisibleComponent loadingHorse;
    
    protected override void OnSetEntity()
    {
        loadingHorse.SetEntity(this.entity.loadingHorse);
    }
}
