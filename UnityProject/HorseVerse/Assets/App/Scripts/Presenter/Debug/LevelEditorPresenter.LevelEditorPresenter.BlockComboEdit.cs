using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public partial class LevelEditorPresenter
{
    private (MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, int index) currentSelectingBlockCombo;
    private Platform currentEditingPlatformObject;
    private readonly List<GameObject> pinList = new List<GameObject>();

    private void OnEditBlockComboBtn()
    {
        UpdateEditMode(UIDebugLevelEditorMode.Mode.BlockCombo);
        SetEntityBlockComboList();
    }

    private void SetEntityBlockComboList()
    {
        uiDebugLevelEditor.entity.blockComboList = new UIDebugLevelEditorBlockListContainer.Entity()
        {
            blockList = masterHorseTrainingBlockComboContainer.DataList.Select((masterHorseTrainingBlockCombo, i) =>
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
        if (!string.IsNullOrEmpty(blockComboName))
        {
            var masterHorseTrainingBlockCombo = new MasterHorseTrainingBlockCombo(masterHorseTrainingBlockComboContainer
                    .MasterHorseTrainingBlockComboIndexer.Max(x => x.Key) + 1, 
                blockComboName);
            masterHorseTrainingBlockComboContainer.Add(masterHorseTrainingBlockCombo);
            UnSelectOldBlock();
            OnEditBlockComboBtn();
            OnEditBlockCombo(masterHorseTrainingBlockCombo, masterHorseTrainingBlockComboContainer.DataList.Length - 1);

            await UniTask.DelayFrame(1);
            uiDebugLevelEditor.blockComboList.blockList.GetComponentsInParent<ScrollRect>().First().normalizedPosition = new Vector2(1, 1);
        }
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
        var ucs = new UniTaskCompletionSource<long>();
        uiDebugLevelEditor.editMode.SetEntity(UIDebugLevelEditorMode.Mode.BlockInCombo);
        uiDebugLevelEditor.entity.blockInComboList = new UIDebugLevelEditorBlockListContainer.Entity()
        {
            blockList = masterHorseTrainingBlockContainer.DataList.Select((masterHorseTrainingBlock, i) =>
                new UIDebugTrainingBlock.Entity()
                {
                    blockName = masterHorseTrainingBlock.Name,
                    selectButtonBtn = new ButtonComponent.Entity(() =>
                    {
                        ucs.TrySetResult(masterHorseTrainingBlock.MasterHorseTrainingBlockId);
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
        var selectingMasterHorseTrainingBlockId = await ucs.Task.AttachExternalCancellation(cts.Token);
        if (selectingMasterHorseTrainingBlockId != default)
        {
            var masterHorseTrainingBlockIdList = currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList.ToList();
            masterHorseTrainingBlockIdList.Add(selectingMasterHorseTrainingBlockId);
            UpdateBlockCombo(masterHorseTrainingBlockIdList);
        }
    }

    private void UpdateBlockCombo(List<long> masterHorseTrainingBlockIdList)
    {
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList =
            masterHorseTrainingBlockIdList.ToArray();
        var master=  currentSelectingBlockCombo.masterHorseTrainingBlockCombo;
        var index = currentSelectingBlockCombo.index;
        currentSelectingBlockCombo = default;
        OnEditBlockCombo(master, index);
    }

    private async UniTask OnDeleteBlockInComBoAsync(MasterHorseTrainingBlock masterHorseTrainingBlock, int index)
    {
        if (await IsUserAgreeTo($"Agree To Delete Block {masterHorseTrainingBlock.Name} \n in {currentSelectingBlockCombo.masterHorseTrainingBlockCombo.Name}?"))
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
        currentEditingPlatformObject = Object.Instantiate(platformPrefab, root.transform);

        currentEditingPlatformObject.GenerateBlocks(Vector3.zero,
            Vector3.zero,
            masterHorseTrainingBlockContainer,
            masterHorseTrainingBlockCombo,
            masterHorseTrainingPropertyContainer.MasterHorseTrainingPropertyIndexer.First().Value);

        currentEditingPlatformObject.GetComponentsInChildren<TrainingMapBlock>()
            .ForEach((x, index) =>
            {
                AddMeshBoundaryDrawer(x.BoundingBoxReference.gameObject);
                AddPintoBlockInCombo(masterHorseTrainingBlockCombo, index, x);
            });

        AddingPinToJumpingPoint();
    }

    private void AddingPinToJumpingPoint()
    {
        var pinGameObject = new GameObject();
        pinGameObject.transform.position = currentEditingPlatformObject.end.position + Vector3.up * 2f;
        pinGameObject.transform.parent = currentEditingPlatformObject.end;

        var pin = CreateUiPin();
        pinList.Add(pin.gameObject);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            isAddBtnVisible = true,
            addBtn = new ButtonComponent.Entity(() => OnAddBlockInComboAsync().Forget()),
            pinTransform = pinGameObject.transform,
            camera = freeCameraComponent
        });
        pin.In().Forget();
    }

    private void AddPintoBlockInCombo(MasterHorseTrainingBlockCombo masterHorseTrainingBlockCombo, int i, TrainingMapBlock x)
    {
        var masterHorseTrainingBlockId = masterHorseTrainingBlockCombo.MasterHorseTrainingBlockIdList[i];
        var masterHorseTrainingBlock = masterHorseTrainingBlockContainer
            .MasterHorseTrainingBlockIndexer[masterHorseTrainingBlockId];
        var blockName = masterHorseTrainingBlock
            .Name;

        var pinTransform = LevelDesignPin.Instantiate(x.BoundingBoxReference.GetComponent<BoxCollider>());
        
        var pin = CreateUiPin();
        pinList.Add(pin.gameObject);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            pinTransform = pinTransform.transform,
            camera = freeCameraComponent,
            blockName = new UIComponentInputField.Entity()
            {
                defaultValue = blockName,
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
            deleteBtn = new ButtonComponent.Entity(() => OnDeleteBlockInComBoAsync(masterHorseTrainingBlock, i).Forget()),
            isDeleteBtnVisible = true
        });
        pin.In().Forget();
    }
    
    public void Swap<T>(List<T> list, int index1, int index2)
    {
        (list[index1], list[index2]) = (list[index2], list[index1]);
    }

    private void UnSelectOldBlockCombo()
    {
        if (currentSelectingBlockCombo != default)
        {
            uiDebugLevelEditor.blockComboList.blockList.instanceList[currentSelectingBlockCombo.index].Select(false);
            currentSelectingBlockCombo = default;
        }

        if (currentEditingPlatformObject != default)
        {
            Object.Destroy(currentEditingPlatformObject.gameObject);
            currentEditingPlatformObject = default;
            
            pinList.ForEach(x => Object.Destroy(x.gameObject));
            pinList.Clear();
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
}