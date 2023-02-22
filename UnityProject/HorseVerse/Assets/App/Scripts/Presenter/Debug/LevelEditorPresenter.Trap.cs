#if ENABLE_DEBUG_MODULE
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public partial class LevelEditorPresenter
{
    private readonly List<(int index, TrapEditor trap)> trapInBlocks = new List<(int index, TrapEditor trap)>();
    private bool isEditingTrap;

    private bool isEditingTrapTarget;
    private bool isEditingTrapDirection;
    private bool isEditingTrapTrigger;

    private readonly List<GameObject> trapPinList = new List<GameObject>();

    private UIDebugLevelEditorTrap.Entity trapEditorEntity = default;
    private TrapEditor currentTrap = default;

    public bool IsEditingTrap
    {
        get => isEditingTrap;
        set
        {
            if (isEditingTrap == value) return;
            isEditingTrap = value;
            OnChangeEditingTrap();
        }
    }

    private bool IsEditingTrapTarget
    {
        get => isEditingTrapTarget;
        set
        {
            if (isEditingTrapTarget == value) return;
            isEditingTrapTarget = value;
            OnChangeEditingTrapTarget();
        }
    }

    private bool IsEditingTrapDirection
    {
        get => isEditingTrapDirection;
        set
        {
            if (isEditingTrapDirection == value) return;
            isEditingTrapDirection = value;
            OnChangeEditingTrapDirection();
        }
    }

    private bool IsEditingTrapTrigger
    {
        get => isEditingTrapTrigger;
        set
        {
            if (isEditingTrapTrigger == value) return;
            isEditingTrapTrigger = value;
            OnChangeEditingTrapTrigger();
        }
    }

    private void OnChangeEditingTrap()
    {
        if (IsEditingTrap)
        {
            GenerateTrap().Forget();
        }
        else
        {
            SaveTrapToBlockAndRemove();
        }

        uiDebugLevelEditor.isAddTrapBtnVisible.SetEntity(IsEditingTrap);
    }

    private void SaveTrapToBlockAndRemove()
    {
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.TrapList = trapInBlocks.Select(x => {
            var trap = trainingBlockSettings.traps[x.index];
            var trapEdi = trap.GetComponent<TrapEditor>();
            return new Trap()
            {
                type = trapEdi.Type.ToString(),
                id = trapEdi.TrapID,
                localPosition = Position.FromVector3(x.trap.transform.localPosition)
            };
        }).ToArray();

        trapInBlocks.ForEach(x => Object.Destroy(x.trap));
        trapInBlocks.Clear();
        trapPinList.ForEach(x => Object.Destroy(x.gameObject));
        trapPinList.Clear();
    }

    private async UniTaskVoid GenerateTrap()
    {
        await UniTask.Yield();
        var traps = trainingBlockSettings.traps.ToList();
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.TrapList
                                  .ForEach(x =>
                                  {
                                      Debug.LogError("Find Trap Name " + $"{x.type}_{x.id}");
                                      CreateTrapAtPosition(
                                          traps.FindIndex(saveTraps => saveTraps.name == $"{x.type}_{x.id}"),
                                          x.localPosition.ToVector3());
                                  });
    }

    private void CreateNewTrap()
    {
        var trapDummy = CreatTrap(0);
        AddPinToTrap(trapDummy, 0);
    }

    private void CreateTrapAtPosition(int index, Vector3 localPosition)
    {
        var trap = CreatTrap(index);
        trap.transform.localPosition = new Vector3(localPosition.x, trap.transform.localPosition.y, localPosition.z);
        AddPinToTrap(trap, index);
    }

    private void CloneTrap(TrapEditor trap, int index)
    {
        var cloned = Object.Instantiate(trap);
        cloned.transform.Cast<Transform>()
              .ForEach(x => Object.Destroy(x.gameObject));
        trapInBlocks.Add((index, cloned));
        AddPinToTrap(cloned, index);
    }

    private TrapEditor CreatTrap(int index)
    {
        var currentPlatformObject = (PlatformModular)currentEditingPlatformObject;
        if (trainingBlockSettings.traps.Length == 0)
        {
            Debug.LogError("Null Trap Editor");
            return null;
        }
        Debug.LogError("Trap Editor index " + index);
        var prefab = trainingBlockSettings.traps[index]
                             .transform;
        var trapDummy = UnityEngine.Object.Instantiate(prefab, currentPlatformObject.transform).gameObject;
        trapDummy.name = prefab.name;
        var runtimeTransformHandle = trapDummy.AddComponent<RuntimeTransformHandle>();
        runtimeTransformHandle.axes = HandleAxes.XZ;
        var trapEditor = trapDummy.GetComponent<TrapEditor>();
        PlatformModular.Snap(currentPlatformObject.FirstCollider, trapDummy.GetComponent<Collider>());
        trapInBlocks.Add((index, trapEditor));

        return trapEditor;
    }

    private void AddPinToTrap(TrapEditor trap,
                                  int index)
    {
        var pin = CreateUiPin(trapPinList);
        pin.SetEntity(new UIDebugLevelDesignBlockTransformPin.Entity()
        {
            isDeleteBtnVisible = true,
            deleteBtn = new ButtonComponent.Entity(() =>
            {
                RemovePin(pin);
                trapInBlocks.RemoveAll(x => x.trap == trap);
                Object.Destroy(trap.gameObject);
            }),
            shuffleBtn = new ButtonComponent.Entity(() =>
            {
                RemovePin(pin);
                Object.Destroy(trap.gameObject);
                trapInBlocks.RemoveAll(x => x.trap == trap);
                ChangeTrap(trap.name, trap.transform.localPosition);
            }),
            isShuffleBtnVisible = true,
            pinTransform = LevelDesignPin.Instantiate(trap.GetComponent<Collider>()).transform,
            camera = freeCameraComponent,
            isDuplicateBtnVisible = true,
            duplicateBtn = new ButtonComponent.Entity(() =>
            {
                CloneTrap(trap, index);
            }),
            isSelectBtnVisible = true,
            selectedBtn = new ButtonComponent.Entity(()=>
            {
                currentTrap = trap;
                uiDebugLevelEditor.isTrapEditorVisible.SetEntity(true);
                uiDebugLevelEditor.trapEditor.SetEntity(GetEntity());
            })
        });
        
        pin.In().Forget();
    }

    private void ChangeTrap(string trapName,
                                Vector3 localPosition)
    {
        var trapIndex = trainingBlockSettings.traps.ToList()
                                                 .FindIndex(x => x.transform.Cast<Transform>()
                                                                  .Any(child => child.gameObject.name == trapName));
        trapIndex = (trapIndex + 1) % trainingBlockSettings.traps.Length;
        CreateTrapAtPosition(trapIndex, localPosition);
    }

    private UIDebugLevelEditorTrap.Entity GetEntity()
    {
        trapEditorEntity = new UIDebugLevelEditorTrap.Entity()
        {
            editTargetToggle = new UIComponentToggle.Entity()
            {
                isOn = false,
                onActiveToggle = val => IsEditingTrapTarget = val
            },
            editDirectionToggle = new UIComponentToggle.Entity()
            {
                isOn = false,
                onActiveToggle = val => IsEditingTrapDirection = val
            },
            editTriggerToggle = new UIComponentToggle.Entity()
            {
                isOn = false,
                onActiveToggle = val => IsEditingTrapTrigger = val
            },
            extraBtn = new ButtonComponent.Entity(() => OnExtraBtnClicked()),

        };
        return trapEditorEntity;
    }

    private void OnChangeEditingTrapTarget()
    {
        if (IsEditingTrapTarget)
        {
            if (currentTrap == default) return;
            if (currentTrap.Type == TrapEditor.TYPE.ROLLING_ROCK)
            {
                var comp = currentTrap.gameObject.GetComponent<TrapEditorRollingStone>();
                comp.RestoreToDefault();
            }
        }
        else
        {
            
        }

        
    }

    private void OnChangeEditingTrapDirection()
    {
        if (IsEditingTrapDirection)
        {
            if (currentTrap == default) return;
            if (currentTrap.Type == TrapEditor.TYPE.ROLLING_ROCK)
            {
                currentTrap.gameObject.GetComponent<TrapEditorRollingStone>().ActiveDirection();
            }
        }
        else
        {

        }


    }

    private void OnChangeEditingTrapTrigger()
    {
        if (IsEditingTrapTrigger)
        {
            if (currentTrap == default) return;
            if (currentTrap.Type == TrapEditor.TYPE.ROLLING_ROCK)
            {
                currentTrap.gameObject.GetComponent<TrapEditorRollingStone>().ActiveTrigger();
            }
        }
        else
        {

        }


    }

    private void OnExtraBtnClicked()
    {
        if (currentTrap == default) return;
        if (currentTrap.Type == TrapEditor.TYPE.ROLLING_ROCK)
        {
            if (IsEditingTrapTarget)
            {
                Debug.Log("1");
            }
            else if (IsEditingTrapDirection)
            {
                Debug.Log("2");
            }
            else if (IsEditingTrapTrigger)
            {
                Debug.Log("3");
            }
        }
    }

}
#endif