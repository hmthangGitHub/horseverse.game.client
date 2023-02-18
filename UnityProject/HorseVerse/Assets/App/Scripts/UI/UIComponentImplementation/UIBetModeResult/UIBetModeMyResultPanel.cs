using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class UIBetModeMyResultPanel : UIComponent<UIBetModeMyResultPanel.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public UIComponentBetModeMyResultList.Entity betModeMyResultList;
        public int horseNumberFirst;
        public string horseNameFirst;
        public int horseNumberSecond;
        public string horseNameSecond;
    }

    public CanvasGroup canvasGroup;
    public UIComponentBetModeMyResultList betModeMyResultList;
    public UIComponentBetSlotNumber horseNumberFirst;
    public FormattedTextComponent horseNameFirst;
    public UIComponentBetSlotNumber horseNumberSecond;
    public FormattedTextComponent horseNameSecond;

    protected override void OnSetEntity()
    {
        betModeMyResultList.SetEntity(this.entity.betModeMyResultList);
        horseNumberFirst.SetEntity(this.entity.horseNumberFirst);
        horseNameFirst.SetEntity(this.entity.horseNameFirst);
        horseNumberSecond.SetEntity(this.entity.horseNumberSecond);
        horseNameSecond.SetEntity(this.entity.horseNameSecond);
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
