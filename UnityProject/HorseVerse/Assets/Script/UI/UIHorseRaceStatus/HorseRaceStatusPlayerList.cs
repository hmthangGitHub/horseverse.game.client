using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HorseRaceStatusPlayerList : UIComponent<HorseRaceStatusPlayerList.Entity>
{
    [Serializable]
    public class Entity
    {
        public int[] horseIdInLane;
        public int playerIndex;
    }

    public RectTransform[] predefinePositions;
    public HorseRaceStatusPlayer template;
    public List<HorseRaceStatusPlayer> horseRaceStatusPlayerList = new List<HorseRaceStatusPlayer>();
    public float distanceToMove = 0.0f;
    public Vector2 destination = Vector2.zero;
    private Vector2 originalPos;
    public RectTransform rectTransform;

    private void Awake()
    {
        template.gameObject.SetActive(false);
        originalPos = rectTransform.anchoredPosition;
    }

    protected override void OnSetEntity()
    {
        CleanOldList();
        
        for (int i = 0; i < this.entity.horseIdInLane.Length; i++)
        {
            var instance = GameObject.Instantiate<HorseRaceStatusPlayer>(template, template.transform.parent, false);
            instance.gameObject.SetActive(true);
            instance.SetEntity(new HorseRaceStatusPlayer.Entity()
            {
                lane = this.entity.horseIdInLane[i] + 1,
                isPlayer = this.entity.horseIdInLane[i] == this.entity.playerIndex,
            });
            horseRaceStatusPlayerList.Add(instance);
        }

        for (int i = 0; i < this.entity.horseIdInLane.Length; i++)
        {
            ChangePosition(this.entity.horseIdInLane[i], this.entity.horseIdInLane[i]);
        }

        RectTransform lastPlayer = predefinePositions[(this.entity.horseIdInLane.Length - 1)];
        distanceToMove = lastPlayer.anchoredPosition.x - lastPlayer.rect.width / 2;
        destination = originalPos - new Vector2(distanceToMove, 0);
    }

    private void CleanOldList()
    {
        foreach (Transform item in template.transform.parent)
        {
            if (item != template.transform)
            {
                Destroy(item.gameObject);
            }
        }
        horseRaceStatusPlayerList.Clear();
    }

    public void ChangePosition(int lane, int toPosition)
    {
        var rectTransfrom = horseRaceStatusPlayerList[lane].transform as RectTransform;
        rectTransfrom.DOKill(false);
        var destination = predefinePositions[(this.entity.horseIdInLane.Length - 1) - toPosition].anchoredPosition;
        rectTransfrom.DOAnchorPos(destination, 0.25f).SetEase(Ease.InOutSine);
    }

    public void UpdatePosition(float normalizePos)
    {
        rectTransform.anchoredPosition = Vector2.Lerp(originalPos, destination, normalizePos);
    }
}
