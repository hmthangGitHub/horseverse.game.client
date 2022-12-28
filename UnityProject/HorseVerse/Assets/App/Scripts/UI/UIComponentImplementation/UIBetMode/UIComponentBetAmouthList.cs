using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UIComponentBetAmouthList : UIComponent<UIComponentBetAmouthList.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public int[] betAmouthList;
        public Action<int> OnFocusIndex = _ => { };
    }
    
    public List<UIComponentBetAmouth> BetAmouths;
    public int currentFocusIndex = -1;

    protected override void OnSetEntity()
    {
        Debug.Log("Set BET AMOUN " + this.entity.betAmouthList.Length);
        currentFocusIndex = -1;
        for(int i = 0; i < BetAmouths.Count; i++)
        {
            int index = i;
            BetAmouths[i].SetEntity(new UIComponentBetAmouth.Entity() { betAmouth = this.entity.betAmouthList[i], button = new ButtonSelectedComponent.Entity(()=> ChangeFocus(index), false)});
        }
        delayToChangeFocus().Forget();
    }


    private void SetButtonsState()
    {
        
    }
    
    async UniTask delayToChangeFocus()
    {
        await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());
        ChangeFocus(0);
    }

    private void ChangeFocus(int index)
    {
        if (index == currentFocusIndex || index >= BetAmouths.Count) return;
        if (currentFocusIndex > -1)
        {
            BetAmouths[currentFocusIndex].button.SetSelected(false);
        }
        currentFocusIndex = index;
        BetAmouths[currentFocusIndex].button.SetSelected(true);
        this.entity.OnFocusIndex(index);
        SetButtonsState();
        
    }
}
