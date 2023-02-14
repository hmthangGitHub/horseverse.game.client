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
        public UIComponentBetModeResultList.Entity betModeResultList;
        public int horseNumberFirst;
        public int horseNameFirst;
        public int horseNumberSecond;
        public int horseNameSecond;
    }

    public CanvasGroup canvasGroup;
    public UIComponentBetModeResultList betModeResultList;

    protected override void OnSetEntity()
    {
        betModeResultList.SetEntity(this.entity.betModeResultList);
    }

    public async UniTask In()
    {
        try
        {
            gameObject.SetActive(true);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, this.GetCancellationTokenOnDestroy());
            DefaultIn();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {

        }
    }

    private void DefaultIn()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public async UniTask Out()
    {
        try
        {
            this.gameObject.SetActive(false);
            DefaultOut();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
        }
    }

    private void DefaultOut()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void Awake()
    {
        DefaultOut();
    }

}
