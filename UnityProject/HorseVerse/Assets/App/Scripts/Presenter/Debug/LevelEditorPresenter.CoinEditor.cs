using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public partial class LevelEditorPresenter
{
    private readonly List<GameObject> coinPinList = new List<GameObject>();
    private bool isEditingCoin;
    private readonly List<CoinEditor> coinEditors = new List<CoinEditor>();

    private void CreateNewCoinEditor()
    {
        var coinEditor = CreateCoinEditor();
        coinEditor.Init(0, Array.Empty<Vector3>());
    }
    
    private async UniTaskVoid GenerateCoinEditors()
    {
        await UniTask.Yield();
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.CoinList.ForEach(CreateCoinEditor);
    }
    
    private void CreateCoinEditor(Coin coin)
    {
        var coinEditor = CreateCoinEditor();
        var coinEditorTransform = coinEditor.transform;
        coinEditorTransform.localPosition = new Vector3(coin.localPosition.x, coinEditorTransform.localPosition.y, coin.localPosition.z);
        coinEditor.Init(coin.numberOfCoin, coin.benzierPointPositions.Select(x => x.ToVector3()).ToArray());
    }

    private CoinEditor CreateCoinEditor()
    {
        var currentPlatformObject = (PlatformModular)currentEditingPlatformObject;
        var coinEditor = Object.Instantiate(levelEditorManager.coinEditor, currentEditingPlatformObject.transform);
        coinEditors.Add(coinEditor);
        coinEditor.transform.localPosition = Vector3.zero;
        Snap(currentPlatformObject.PaddingHeadCollider, coinEditor.GetComponent<Collider>());
        AddPintoCoinEditor(coinEditor);
        return coinEditor;
    }

    private void AddPintoCoinEditor(CoinEditor coinEditor)
    {
        var pin = CreateUiPin(obstaclePinList);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            isDeleteBtnVisible = true,
            deleteBtn = new ButtonComponent.Entity(() =>
            {
                Object.Destroy(coinEditor.gameObject);
                coinEditors.Remove(coinEditor);
                RemovePin(pin);
            }),
            pinTransform = LevelDesignPin.Instantiate(coinEditor.GetComponent<Collider>()).transform,
            camera = freeCameraComponent
        });
        pin.In().Forget();
    }

    private bool IsEditingCoin
    {
        get => isEditingCoin;
        set
        {
            if (isEditingCoin == value) return;
            isEditingCoin = value;
            OnChangeEditingCoin();
        }
    }

    private void OnChangeEditingCoin()
    {
        if (IsEditingCoin == false)
        {
            SaveCoinsToBlockAndRemove();
        }
        else
        {
            currentSelectingBlockCombo.masterHorseTrainingBlockCombo.CoinList.ForEach(CreateCoinEditor);
        }
        
        uiDebugLevelEditor.isAddCoinBtnVisible.SetEntity(IsEditingCoin);
    }

    private void SaveCoinsToBlockAndRemove()
    {
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.CoinList = coinEditors.Select(x => new Coin()
            {
                localPosition = Position.FromVector3(x.transform.localPosition),
                numberOfCoin = x.CoinNumber,
                benzierPointPositions = x.BenzierPointPositions.Select(Position.FromVector3)
                                         .ToArray()
            })
            .ToArray();
        coinEditors.ForEach(x => Object.Destroy(x.gameObject));
        coinEditors.Clear();
    }
}