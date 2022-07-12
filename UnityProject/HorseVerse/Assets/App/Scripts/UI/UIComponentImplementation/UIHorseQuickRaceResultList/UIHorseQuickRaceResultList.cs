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
            0 => UIComponentQuickRaceResultType.ResultType.first,
            1 => UIComponentQuickRaceResultType.ResultType.second,
            2 => UIComponentQuickRaceResultType.ResultType.third,
            _ => UIComponentQuickRaceResultType.ResultType.other
        };
    }

    private void OnEnable()
    {
        RectTransform layoutRoot = transform as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
        layoutRoot.ForceUpdateRectTransforms();
        TryEnableLayoutBuilder().Forget();
    }

    protected async override UniTask AnimationIn()
    {
        await base.AnimationIn();
        TryEnableLayoutBuilder().Forget();
    }

    private async UniTaskVoid TryEnableLayoutBuilder()
    {
        await UniTask.DelayFrame(2,cancellationToken: this.GetCancellationTokenOnDestroy());
        this.quickRaceResultList.gameObject.SetActive(false);
        this.quickRaceResultList.gameObject.SetActive(true);
        await UniTask.Yield(this.GetCancellationTokenOnDestroy());  
        this.quickRaceResultList.gameObject.SetActive(false);
        this.quickRaceResultList.gameObject.SetActive(true);
    }
}	