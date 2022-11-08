using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public partial class LevelEditorPresenter
{
    private (MasterHorseTrainingBlock masterHorseTrainingBlock, int i) currentSelectedBlock;
    private TrainingMapBlock currentEditingTrainingMapBlockGameObject;
    
    private void OnEditBlockBtn()
    {
        UpdateEditMode(UIDebugLevelEditorMode.Mode.Block);
        uiDebugLevelEditor.entity.blockList = new UIDebugLevelEditorBlockListContainer.Entity()
        {
            blockList = masterHorseTrainingBlockContainer.DataList.Select((masterHorseTrainingBlock, i) =>
                new UIDebugTrainingBlock.Entity()
                {
                    blockName = masterHorseTrainingBlock.Name,
                    deleteBtn = new ButtonComponent.Entity(() => OnDeleteBlockAsync(masterHorseTrainingBlock).Forget()),
                    selectButtonBtn = new ButtonComponent.Entity(() => { OnEditBlock(i, masterHorseTrainingBlock); })
                }).ToArray(),
            addBtn = new ButtonComponent.Entity(() => OnAddBlockAsync().Forget())
        };
        uiDebugLevelEditor.blockList.SetEntity(uiDebugLevelEditor.entity.blockList);
    }

    private async UniTask OnAddBlockAsync()
    {
        var name = await AskUserInput("Enter Block Name");
        if (!string.IsNullOrEmpty(name))
        {
            var masterHorseTrainingBlock = new MasterHorseTrainingBlock(
                masterHorseTrainingBlockContainer.DataList.Max(x => x.MasterHorseTrainingBlockId) + 1, name);
            masterHorseTrainingBlockContainer.Add(masterHorseTrainingBlock);
            OnEditBlockBtn();
            OnEditBlock(masterHorseTrainingBlockContainer.DataList.Length - 1, masterHorseTrainingBlock);
            await UniTask.DelayFrame(1);
            uiDebugLevelEditor.blockList.blockList
                .GetComponentsInParent<ScrollRect>()
                .First().normalizedPosition = new Vector2(1, 1);
        }
    }

    private void OnEditBlock(int i, MasterHorseTrainingBlock masterHorseTrainingBlock)
    {
        if (currentSelectedBlock != (masterHorseTrainingBlock, i))
        {
            UnSelectOldBlock();
        }

        uiDebugLevelEditor.blockList.blockList.instanceList[i].Select(true);
        currentSelectedBlock = (masterHorseTrainingBlock, i);
        currentEditingTrainingMapBlockGameObject = Object.Instantiate(trainingMapBlockPrefab, root.transform);
        currentEditingTrainingMapBlockGameObject.BoundingBoxReference.GetComponent<MeshRenderer>().enabled = true;
        DrawBlockBoundingBoxDebugLines();
        AddPinToEachBlockSegment();
    }

    private void AddPinToEachBlockSegment()
    {
        var blockNamePinTransform = LevelDesignPin.Instantiate(currentEditingTrainingMapBlockGameObject.BoundingBoxReference.gameObject.GetComponent<BoxCollider>());
        blockNamePin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            blockName = new UIComponentInputField.Entity()
            {
                defaultValue = currentSelectedBlock.masterHorseTrainingBlock.Name
            },
            isBlockNameVisible = true,
            camera = freeCameraComponent,
            pinTransform = blockNamePinTransform.transform,
        });
        blockNamePin.In().Forget();
        currentEditingTrainingMapBlockGameObject.Lanes.ForEach((x, i) =>
        {
            x.GenBlockLane(currentSelectedBlock.masterHorseTrainingBlock[i]);
            SetPinEntities(x, i, freeCameraComponent);
        });
    }

    private void SetPinEntities(TrainingBlockLane x, int i, Camera currentCamera)
    {
        var pin = LevelDesignPin.Instantiate(x.gameObject.GetComponent<BoxCollider>());
        blockSegmentPin[i].SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            coinNumber = new UIComponentInputField.Entity()
            {
                defaultValue = currentSelectedBlock.masterHorseTrainingBlock[i].customValue.ToString(),
                onValueChange = value =>
                {
                    if (Int32.TryParse(value, out int numberOfCoin))
                    {
                        var info = currentSelectedBlock.masterHorseTrainingBlock[i];
                        info.customValue = numberOfCoin;
                        UpdateLaneSegment(x, i, currentCamera, info);
                    }
                }
            },
            pinTransform = pin.transform,
            camera = currentCamera,
            shuffleBtn = new ButtonComponent.Entity(() =>
            {
                var enumLength = Enum.GetValues(typeof(MasterHorseTrainingLaneType)).Length;
                var info = currentSelectedBlock.masterHorseTrainingBlock[i];
                info.laneType = (MasterHorseTrainingLaneType)((int)(info.laneType + 1) % enumLength);
                UpdateLaneSegment(x, i, currentCamera, info);
            }),
            isCoinNumberVisible = currentSelectedBlock.masterHorseTrainingBlock[i].laneType ==
                                  MasterHorseTrainingLaneType.Coin ||
                                  currentSelectedBlock.masterHorseTrainingBlock[i].laneType ==
                                  MasterHorseTrainingLaneType.JumpCoin,
            isShuffleBtnVisible = true,
            isBlockNameVisible = false,
        });
        blockSegmentPin[i].In().Forget();
    }

    private void UpdateLaneSegment(TrainingBlockLane x, int i, Camera currentCamera,
        (MasterHorseTrainingLaneType laneType, int customValue) info)
    {
        currentSelectedBlock.masterHorseTrainingBlock[i] = info;
        x.GenBlockLane(info);
        SetPinEntities(x, i, currentCamera);
    }

    private void UnSelectOldBlock()
    {
        if (currentEditingTrainingMapBlockGameObject != default)
        {
            uiDebugLevelEditor.blockList.blockList.instanceList[currentSelectedBlock.i].Select(false);
            Object.Destroy(currentEditingTrainingMapBlockGameObject.gameObject);
            currentEditingTrainingMapBlockGameObject = default;
            blockNamePin.Out().Forget();
            blockSegmentPin.ForEach(x => x.Out().Forget());
        }
    }

    private async UniTask LoadBlockEditUIAssetsAsync()
    {
        blockSegmentPin = await UniTask.WhenAll(new[]
        {
            UILoader.Instantiate<UIDebugLevelDesignBlockTransformPin>(token: cts.Token),
            UILoader.Instantiate<UIDebugLevelDesignBlockTransformPin>(token: cts.Token),
            UILoader.Instantiate<UIDebugLevelDesignBlockTransformPin>(token: cts.Token),
        });
        blockNamePin = await UILoader.Instantiate<UIDebugLevelDesignBlockTransformPin>(token: cts.Token);
    }

    private async UniTaskVoid OnDeleteBlockAsync(MasterHorseTrainingBlock x)
    {
        if (await IsUserAgreeTo($"Delete block {x.Name}?"))
        {
            masterHorseTrainingBlockContainer.Remove(x.MasterHorseTrainingBlockId);
            OnEditBlockBtn();
        }
    }

    private void DrawBlockBoundingBoxDebugLines()
    {
        AddMeshBoundaryDrawer(currentEditingTrainingMapBlockGameObject.BoundingBoxReference.gameObject);
        currentEditingTrainingMapBlockGameObject.Lanes.ForEach(x =>
        {
            AddMeshBoundaryDrawer(x.gameObject);
        });
    }
}