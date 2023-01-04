using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public partial class LevelEditorPresenter
{
    private (MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, int index) currentSelectingBlockCombo;
    private PlatformBase currentEditingPlatformObject;
    private readonly List<GameObject> blockComboPinList = new List<GameObject>();
    private MasterHorseTrainingProperty masterHorseTrainingProperty;
    private bool isEditingBlock;

    public bool IsEditingBlock
    {
        get => isEditingBlock;
        set
        {
            if (isEditingBlock == value) return;
            isEditingBlock = value;
            OnChangeEditingBlock();
        }
    }

    private void OnChangeEditingBlock()
    {
        if (IsEditingBlock == false)
        {
            RemoveAllBlockPins();
        }
        else
        {
            AddPinToBlock(((PlatformModular)currentEditingPlatformObject));
        }
    }

    private void OnEditBlockComboBtn()
    {
        UpdateEditMode(UIDebugLevelEditorMode.Mode.BlockCombo);
        SetEntityBlockComboList();
    }

    private void SetEntityBlockComboList()
    {
        uiDebugLevelEditor.entity.blockComboList = new UIDebugLevelEditorBlockListContainer.Entity()
        {
            blockList = masterHorseTrainingBlockComboContainer.DataList
                                                              .Where(x => x.MasterTrainingBlockComboType == CurrentBlockComboType)
                                                              .Select((masterHorseTrainingBlockCombo, i) =>
                new UIDebugTrainingBlock.Entity()
                {
                    blockName = masterHorseTrainingBlockCombo.Name,
                    deleteBtn = new ButtonComponent.Entity(() =>
                        OnDeleteBlockComboAsync(masterHorseTrainingBlockCombo).Forget()),
                    selectButtonBtn = new ButtonComponent.Entity(() => OnEditBlockCombo(masterHorseTrainingBlockCombo, i))
                }).ToArray(),
            addBtn = new ButtonComponent.Entity(() => OnAddBlockComboAsync().Forget())
        };
        uiDebugLevelEditor.blockComboList.SetEntity(uiDebugLevelEditor.entity.blockComboList);
    }

    private async UniTaskVoid OnAddBlockComboAsync()
    {
        var blockComboName = await AskUserInput("Enter block combo name");
        if (string.IsNullOrEmpty(blockComboName)) return;
        var masterHorseTrainingBlockId = CurrentBlockComboType == MasterTrainingBlockComboType.Predefine
            ? await SelectingMasterHorseTrainingBlockIdAsync()
            : string.Empty;
        if (CurrentBlockComboType == MasterTrainingBlockComboType.Predefine &&
            string.IsNullOrEmpty(masterHorseTrainingBlockId)) return;
        
        var masterHorseTrainingBlockComboId = masterHorseTrainingBlockComboContainer
                                         .MasterHorseTrainingBlockComboIndexer.Max(x => x.Key) + 1;
        var masterHorseTrainingBlockCombo = new MasterHorseTrainingBlockCombo(masterHorseTrainingBlockComboId,
            blockComboName,
            CurrentBlockComboType,
            masterHorseTrainingBlockId);
        masterHorseTrainingBlockComboContainer.Add(masterHorseTrainingBlockCombo);
        UnSelectOldBlock();
        OnEditBlockComboBtn();
        OnEditBlockCombo(masterHorseTrainingBlockCombo, masterHorseTrainingBlockComboContainer.DataList
            .Where(x => x.MasterTrainingBlockComboType == CurrentBlockComboType)
            .ToArray()
            .Length - 1);

        await UniTask.DelayFrame(1);
        uiDebugLevelEditor.blockComboList.blockList.GetComponentsInParent<ScrollRect>()
                          .First()
                          .normalizedPosition = new Vector2(1, 1);
    }

    private void OnEditBlockCombo(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, int i)
    {
        if (currentSelectingBlockCombo != (masterHorseTrainingBlockCombo, i))
        {
            UnSelectOldBlockCombo();
        }
        CreatePlatform(i, masterHorseTrainingBlockCombo);
    }

    private async UniTask OnAddBlockInComboAsync()
    {
        var selectingMasterHorseTrainingBlockId = await SelectingMasterHorseTrainingBlockIdAsync();
        if (selectingMasterHorseTrainingBlockId != default)
        {
            var masterHorseTrainingBlockIdList = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList.ToList();
            masterHorseTrainingBlockIdList.Add(selectingMasterHorseTrainingBlockId);
            UpdateBlockCombo(masterHorseTrainingBlockIdList);
        }
    }

    private async UniTask<string> SelectingMasterHorseTrainingBlockIdAsync()
    {
        var ucs = new UniTaskCompletionSource<string>();
        uiDebugLevelEditor.editMode.SetEntity(UIDebugLevelEditorMode.Mode.BlockInCombo);
        uiDebugLevelEditor.entity.blockInComboList = new UIDebugLevelEditorBlockListContainer.Entity()
        {
            blockList = masterTrainingModularBlockContainer.DataList
                                                           .Where(x => x.MasterTrainingModularBlockType == FromBlockComboTypeToModularBlockType(CurrentBlockComboType)).Select((masterHorseTrainingBlock, i) =>
                new UIDebugTrainingBlock.Entity()
                {
                    blockName = masterHorseTrainingBlock.MasterTrainingModularBlockId,
                    selectButtonBtn = new ButtonComponent.Entity(() =>
                    {
                        ucs.TrySetResult(masterHorseTrainingBlock.MasterTrainingModularBlockId);
                        uiDebugLevelEditor.editMode.SetEntity(UIDebugLevelEditorMode.Mode.BlockCombo);
                    }),
                }).ToArray(),
            closeBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult(default);
                uiDebugLevelEditor.editMode.SetEntity(UIDebugLevelEditorMode.Mode.BlockCombo);
            })
        };
        uiDebugLevelEditor.blockInComboList.SetEntity(uiDebugLevelEditor.entity.blockInComboList);
        return await ucs.Task.AttachExternalCancellation(cts.Token);
    }
    
    private void UpdateBlockCombo(List<string> masterHorseTrainingBlockIdList)
    {
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList =
            masterHorseTrainingBlockIdList.ToArray();
        var master=  currentSelectingBlockCombo.masterHorseTrainingBlockCombo;
        var index = currentSelectingBlockCombo.index;
        currentSelectingBlockCombo = default;
        OnEditBlockCombo(master, index);
    }

    private async UniTask OnDeleteBlockInComBoAsync(string name, int index)
    {
        if (await IsUserAgreeTo($"Agree To Delete Block {name} \n " +
                                $"in {currentSelectingBlockCombo.masterHorseTrainingBlockCombo.Name}?"))
        {
            var masterHorseTrainingBlockIdList = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList.ToList();
            masterHorseTrainingBlockIdList.RemoveAt(index);
            UpdateBlockCombo(masterHorseTrainingBlockIdList);
        }
    }

    private void CreatePlatform(int i, MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo)
    {
        uiDebugLevelEditor.blockComboList.blockList.instanceList[i].Select(true);
        currentSelectingBlockCombo = (masterHorseTrainingBlockCombo, i);
        currentEditingPlatformObject = Object.Instantiate(platformPrefab, levelEditorManager.transform);
        
        var paddingBlockId = masterTrainingModularBlockContainer.MasterTrainingModularBlockIndexer.Values
                                                                .First(x => x.MasterTrainingModularBlockType == MasterTrainingModularBlockType.Padding)
                                                                .MasterTrainingModularBlockId;

        var modularBlockIds = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList; 

        var editingPlatformObject = ((PlatformModular)currentEditingPlatformObject);
        editingPlatformObject.GenerateBlock(Vector3.zero, modularBlockIds.Select(x => trainingBlockSettings.BlocksLookUpTable[x].gameObject).ToArray(), 
            trainingBlockSettings.BlocksLookUpTable[paddingBlockId].gameObject,
        trainingBlockSettings.BlocksLookUpTable[paddingBlockId].gameObject,
        masterHorseTrainingProperty.JumpingPoint,
        masterHorseTrainingProperty.LandingPoint,
        CurrentBlockComboType);
        OnChangeEditingBlock();
        
        if (IsEditingObstacle)
        {
            GenerateObstacle().Forget();
        }
        if (IsEditingCoin)
        {
            GenerateCoinEditors().Forget();
        }
    }

    private void RemoveAllBlockPins()
    {
        blockComboPinList.ForEach(x => Object.Destroy(x.gameObject));
        blockComboPinList.Clear();
    }

    private void AddPinToBlock(PlatformModular editingPlatformObject)
    {
        if (CurrentBlockComboType == MasterTrainingBlockComboType.Modular)
        {
            CreateBlockNamePin(editingPlatformObject);
            CreateAddPin(editingPlatformObject);
        }
        editingPlatformObject.BoxColliders.ForEach(AddPinToBlockModular);
    }

    private void CreateAddPin(PlatformModular editingPlatformObject)
    {
        var pin = CreateUiPin(blockComboPinList);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            isAddBtnVisible = true,
            addBtn = new ButtonComponent.Entity(() => OnAddBlockInComboAsync().Forget()),
            pinTransform = LevelDesignPin.Instantiate(editingPlatformObject.PaddingTailCollider).transform,
            camera = freeCameraComponent,
            
        });
        pin.In().Forget();
    }

    private void AddPinToBlockModular(BoxCollider boxCollider,
                                      int i)
    {
        var pin = CreateUiPin(blockComboPinList);
        var masterHorseTrainingBlockId = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList[i];
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            pinTransform = LevelDesignPin.Instantiate(boxCollider).transform,
            camera = freeCameraComponent,
            blockName = new UIComponentInputField.Entity()
            {
                defaultValue = masterHorseTrainingBlockId,
                interactable = false
            },
            isBlockNameVisible = true,
            isNavigationBtnVisible = true,
            leftBtn = new ButtonComponent.Entity(() =>
            {
                var masterHorseTrainingBlockIdList = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList.ToList();
                if (i != masterHorseTrainingBlockIdList
                        .Count - 1)
                {
                    Swap(masterHorseTrainingBlockIdList, i, i + 1);
                    UpdateBlockCombo(masterHorseTrainingBlockIdList);    
                }
            }),
            rightBtn = new ButtonComponent.Entity(() =>
            {
                var masterHorseTrainingBlockIdList = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList.ToList();
                if (i != 0)
                {
                    Swap(masterHorseTrainingBlockIdList, i, i - 1);
                    UpdateBlockCombo(masterHorseTrainingBlockIdList);
                }
            }),
            deleteBtn = new ButtonComponent.Entity(() =>
            {
                var masterBlockModularId = currentSelectingBlockCombo.masterHorseTrainingBlockCombo
                                                                         .MasterHorseTrainingBlockIdList[i]; 
                OnDeleteBlockInComBoAsync(masterBlockModularId, i).Forget();
            }),
            isDeleteBtnVisible = true,
            shuffleBtn = new ButtonComponent.Entity(() => OnChangeToAnotherBlock(i).Forget()),
            isShuffleBtnVisible = true
        });
        pin.In().Forget();
    }

    private void CreateBlockNamePin(PlatformModular editingPlatformObject)
    {
        var pin = CreateUiPin(blockComboPinList);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            pinTransform = LevelDesignPin.Instantiate(editingPlatformObject.PaddingHeadCollider).transform,
            camera = freeCameraComponent,
            blockName = new UIComponentInputField.Entity()
            {
                defaultValue = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.Name,
                interactable = false
            },
            isBlockNameVisible = true
        });
        pin.In().Forget();
    }

    private async UniTaskVoid OnChangeToAnotherBlock(int i)
    {
        var selectingMasterHorseTrainingBlockId = await SelectingMasterHorseTrainingBlockIdAsync();
        if (selectingMasterHorseTrainingBlockId != default)
        {
            var idList = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList;
            idList[i] = selectingMasterHorseTrainingBlockId;
            UpdateBlockCombo(idList.ToList());
        }
    }

    public void Swap<T>(List<T> list, int index1, int index2)
    {
        (list[index1], list[index2]) = (list[index2], list[index1]);
    }

    private void UnSelectOldBlockCombo()
    {
        if (currentSelectingBlockCombo != default)
        {
            if (IsEditingObstacle)
            {
                SaveObstacleToBlockAndRemove();
            }
            if (IsEditingCoin)
            {
                SaveCoinsToBlockAndRemove();
            }
            
            uiDebugLevelEditor.blockComboList.blockList.instanceList[currentSelectingBlockCombo.index].Select(false);
            currentSelectingBlockCombo = default;
        }

        if (currentEditingPlatformObject != default)
        {
            Object.Destroy(currentEditingPlatformObject.gameObject);
            currentEditingPlatformObject = default;
            
            blockComboPinList.ForEach(x => Object.Destroy(x.gameObject));
            blockComboPinList.Clear();
        }
    }

    private async UniTaskVoid OnDeleteBlockComboAsync(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo)
    {
        if (await IsUserAgreeTo($"Agree To Delete Block Combo {masterHorseTrainingBlockCombo.Name}?"))
        {
            masterHorseTrainingBlockComboContainer.Remove(masterHorseTrainingBlockCombo.MasterHorseTrainingBlockId);
            UnSelectOldBlockCombo();
            OnEditBlockComboBtn();
        }
    }

    private void OnSelectBlockComboType()
    {
        UnSelectOldBlockCombo();
        SetEntityBlockComboList();
        uiDebugLevelEditor.editMode.SetEntity(UIDebugLevelEditorMode.Mode.BlockCombo);
    }

    private MasterTrainingModularBlockType FromBlockComboTypeToModularBlockType(
        MasterTrainingBlockComboType masterTrainingBlockComboType)
    {
        return masterTrainingBlockComboType switch
        {
            MasterTrainingBlockComboType.Custom => MasterTrainingModularBlockType.Custom,
            MasterTrainingBlockComboType.Modular => MasterTrainingModularBlockType.Modular,
            MasterTrainingBlockComboType.Predefine => MasterTrainingModularBlockType.Predefine,
            _ => MasterTrainingModularBlockType.Modular,
        };
    }
}