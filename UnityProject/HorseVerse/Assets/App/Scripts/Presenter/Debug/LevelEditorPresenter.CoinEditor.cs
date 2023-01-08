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
    private CoinEditor currentEditingCoin;
    private MasterCoinPresetContainer masterCoinPresetContainer;

    private void CreateNewCoinEditor()
    {
        var coinEditor = CreateCoinEditor();
        coinEditor.Init(0, Array.Empty<Vector3>());
    }
    
    private async UniTaskVoid GenerateCoinEditors()
    {
        await UniTask.Yield();
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.CoinList.ForEach(CreateCoinEditorFromMasterCoin);
    }
    
    private void CreateCoinEditorFromMasterCoin(Coin coin)
    {
        var coinEditor = CreateCoinEditor();
        var coinEditorTransform = coinEditor.transform;
        coinEditorTransform.localPosition = new Vector3(coin.localPosition.x, coinEditorTransform.localPosition.y, coin.localPosition.z);
        coinEditor.Init(coin.numberOfCoin, coin.benzierPointPositions.Select(x => x.ToVector3()).ToArray());
    }
    
    private void CloneCoinEditor(CoinEditor coinEditor)
    {
        var clonedCoinEditor = CreateCoinEditor();
        var coinEditorTransform = clonedCoinEditor.transform;
        coinEditorTransform.localPosition = coinEditor.transform.localPosition;
        clonedCoinEditor.Init(coinEditor.CoinNumber, coinEditor.BenzierPointPositions);
    }

    private CoinEditor CreateCoinEditor()
    {
        var currentPlatformObject = (PlatformModular)currentEditingPlatformObject;
        var coinEditor = Object.Instantiate(levelEditorManager.coinEditor, currentEditingPlatformObject.transform);
        coinEditors.Add(coinEditor);
        coinEditor.transform.localPosition = Vector3.zero;
        PlatformModular.Snap(currentPlatformObject.FirstCollider, coinEditor.GetComponent<Collider>());
        AddPintoCoinEditor(coinEditor);
        return coinEditor;
    }

    private void AddPintoCoinEditor(CoinEditor coinEditor)
    {
        var pin = CreateUiPin(coinPinList);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            isDeleteBtnVisible = true,
            deleteBtn = new ButtonComponent.Entity(() =>
            {
                Object.Destroy(coinEditor.gameObject);
                coinEditors.Remove(coinEditor);
                RemovePin(pin);
            }),
            shuffleBtn = new ButtonComponent.Entity(() =>
            {
                if (currentEditingCoin != default)
                {
                    currentEditingCoin.OnToggleStatus();
                }
                
                currentEditingCoin = coinEditor;
                uiDebugLevelEditor.isCoinEditorVisible.SetEntity(true);
                uiDebugLevelEditor.coinEditor.SetEntity(new UIDebugLevelEditorSplineEditor.Entity()
                {
                    mode = UIComponentSplineEditorMode.Status.Normal,
                    addBtn = new ButtonComponent.Entity(coinEditor.AddNewBenzierPoint),
                    removeBtn = new ButtonComponent.Entity(coinEditor.RemoveLastBenzierPoint),
                    coinNumber = new UIComponentInputField.Entity()
                    {
                        defaultValue = coinEditor.CoinNumber.ToString(),
                        onValueChange = val => coinEditor.OnChangeNumberOfCoin(int.TryParse(val, out var number) ? number : default)
                    },
                    saveToPresetBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
                    {
                        var presetName = await AskUserInput("Enter preset name");
                        if (!string.IsNullOrEmpty(presetName))
                        {
                            masterCoinPresetContainer.AddOrModified(MasterCoinPreset.Instantiate(presetName, FromCoinEditorToMasterCoin(coinEditor)));
                        }
                    }))
                });
                currentEditingCoin.OnToggleStatus();
            }),
            isShuffleBtnVisible = true,
            pinTransform = LevelDesignPin.Instantiate(coinEditor.GetComponent<Collider>()).transform,
            camera = freeCameraComponent,
            duplicateBtn = new ButtonComponent.Entity(() => CloneCoinEditor(coinEditor)),
            isDuplicateBtnVisible = true
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
            currentSelectingBlockCombo.masterHorseTrainingBlockCombo.CoinList.ForEach(CreateCoinEditorFromMasterCoin);
        }
        
        uiDebugLevelEditor.isAddCoinBtnVisible.SetEntity(IsEditingCoin);
        uiDebugLevelEditor.isAddFromPresetVisible.SetEntity(IsEditingCoin);
    }

    private void SaveCoinsToBlockAndRemove()
    {
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.CoinList = coinEditors.Select(FromCoinEditorToMasterCoin)
            .ToArray();
        coinEditors.ForEach(x => Object.Destroy(x.gameObject));
        coinEditors.Clear();
        coinPinList.ForEach(x => Object.Destroy(x.gameObject));
        coinPinList.Clear();
        uiDebugLevelEditor.isCoinEditorVisible.SetEntity(false);
        currentEditingCoin = default;
    }

    private async UniTaskVoid AddCoinFromPresetAsync()
    {
        var masterCoinPresetId = await SelectFromList(masterCoinPresetContainer.MasterCoinPresetIndexer.Keys);
        if (!string.IsNullOrEmpty(masterCoinPresetId))
        {
            CreateCoinEditorFromMasterCoin(masterCoinPresetContainer.MasterCoinPresetIndexer[masterCoinPresetId]
                                                                    .CoinObject);
        }
    }

    private static Coin FromCoinEditorToMasterCoin(CoinEditor x)
    {
        return new Coin()
        {
            localPosition = Position.FromVector3(x.transform.localPosition),
            numberOfCoin = x.CoinNumber,
            benzierPointPositions = x.BenzierPointPositions.Select(Position.FromVector3)
                                     .ToArray()
        };
    }
}