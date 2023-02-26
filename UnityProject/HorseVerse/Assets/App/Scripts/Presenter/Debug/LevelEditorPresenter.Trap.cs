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
            var trap = x.trap;
            var trapEdi = trap.GetComponent<TrapEditor>();
            return new Trap()
            {
                type = trapEdi.Type.ToString(),
                eType = (int)trapEdi.Type,
                id = trapEdi.TrapID,
                localPosition = Position.FromVector3(x.trap.transform.localPosition),
                extraData = trapEdi.getExtraData(),
            };
        }).ToArray();

        trapInBlocks.ForEach(x => Object.Destroy(x.trap.gameObject));
        trapInBlocks.Clear();
        trapPinList.ForEach(x => Object.Destroy(x.gameObject));
        trapPinList.Clear();

        uiDebugLevelEditor.isTrapEditorVisible.SetEntity(false);
        currentTrap = default;
    }

    private async UniTaskVoid GenerateTrap()
    {
        await UniTask.Yield();
        var traps = trainingBlockSettings.trapEditors.ToList();
        currentSelectingBlockCombo.masterHorseTrainingBlockCombo.TrapList
                                  .ForEach(x =>
                                  {
                                      Debug.LogError("Find Trap Name " + $"{x.type}_{x.id}");
                                      CreateTrapAtPosition(
                                          traps.FindIndex(saveTraps => saveTraps.name == $"{x.type}_{x.id}"),
                                          x.localPosition.ToVector3(),x.extraData);
                                  });
    }

    private void CreateNewTrap()
    {
        var trapDummy = CreatTrap(0);
        AddPinToTrap(trapDummy, 0);
    }

    private void CreateTrapAtPosition(int index, Vector3 localPosition, string extraData = "")
    {
        var trap = CreatTrap(index);
        trap.transform.localPosition = new Vector3(localPosition.x, trap.transform.localPosition.y, localPosition.z);
        if (!string.IsNullOrEmpty(extraData))
        {
            if (trap.Type == TrapEditor.TYPE.ROLLING_ROCK)
            {
                var comp = trap.gameObject.GetComponent<TrapEditorRollingStone>();
                comp.SetExtraData(extraData);
            }
            else if (trap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
            {
                var comp = trap.gameObject.GetComponent<TrapEditorWoodSpike>();
                comp.SetExtraData(extraData);
            }
        }
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
        if (trainingBlockSettings.trapEditors.Length == 0)
        {
            Debug.LogError("Null Trap Editor");
            return null;
        }
        Debug.LogError("Trap Editor index " + index);
        var prefab = trainingBlockSettings.trapEditors[index]
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
                uiDebugLevelEditor.isTrapEditorVisible.SetEntity(false);
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
                if(currentTrap != trap)
                { 
                    currentTrap = trap;
                    uiDebugLevelEditor.isTrapEditorVisible.SetEntity(true);
                    uiDebugLevelEditor.trapEditor.SetEntity(GetEntity());
                }
                else
                {
                    uiDebugLevelEditor.isTrapEditorVisible.SetEntity(false);
                    currentTrap = default;
                }
            })
        });
        
        pin.In().Forget();
    }

    private void ChangeTrap(string trapName,
                                Vector3 localPosition)
    {
        var trapIndex = trainingBlockSettings.trapEditors.ToList().FindIndex(saveTraps => saveTraps.name == trapName);
        //var trapIndex = trainingBlockSettings.trapEditors.ToList()
        //                                         .FindIndex(x => x.transform.Cast<Transform>()
        //                                                          .Any(child => child.gameObject.name == trapName));
        trapIndex = (trapIndex + 1) % trainingBlockSettings.trapEditors.Length;
        CreateTrapAtPosition(trapIndex, localPosition);
    }

    private UIDebugLevelEditorTrap.Entity GetEntity()
    {
        var strapEditorEntity = new UIDebugLevelEditorTrap.Entity()
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
            btnAddChild = new ButtonComponent.Entity(() => OnAddChild()),
            btnDeleteChild = new ButtonComponent.Entity(() => OnDeleteChild()),
        };
        return strapEditorEntity;
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
            else if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
            {
                var comp = currentTrap.gameObject.GetComponent<TrapEditorWoodSpike>();
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
            else if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
            {
                if (uiDebugLevelEditor.trapEditor.entity.isVisibleChildrenPanel)
                    uiDebugLevelEditor.trapEditor.isVisibleChildrenPanel.SetEntity(false);
                currentTrap.gameObject.GetComponent<TrapEditorWoodSpike>().ActiveDirection();
            }
        }
        else
        {
            if (currentTrap == default) return;
            if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
            {
                if (uiDebugLevelEditor.trapEditor.entity.isVisibleChildrenPanel)
                    uiDebugLevelEditor.trapEditor.isVisibleChildrenPanel.SetEntity(false);
            }
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
            else if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
            {
                currentTrap.gameObject.GetComponent<TrapEditorWoodSpike>().ActiveTrigger();
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
        else if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
        {
            
            if (IsEditingTrapDirection)
            {
                if (uiDebugLevelEditor.trapEditor.entity.isVisibleChildrenPanel)
                    uiDebugLevelEditor.trapEditor.SetVisiblePanelChildren(false);
                else
                {
                    uiDebugLevelEditor.trapEditor.SetVisiblePanelChildren(true);
                    var comp = currentTrap.GetComponent<TrapEditorWoodSpike>();
                    uiDebugLevelEditor.trapEditor.SetChildren(new UIDebugLevelEditorTrapToggleList.Entity()
                    {
                        entities = GetListToggleWoodSpike(comp)
                    });

                }
                
            }
            else
            {
                if (uiDebugLevelEditor.trapEditor.entity.isVisibleChildrenPanel)
                    uiDebugLevelEditor.trapEditor.SetVisiblePanelChildren(false);
            }

        }

    }

    private UIDebugLevelEditorTrapToggleItem.Entity[] GetListToggleWoodSpike(TrapEditorWoodSpike comp)
    {
        var x = comp.Points.Select(x => new UIDebugLevelEditorTrapToggleItem.Entity()
        {
            toggle = new UIComponentToggle.Entity()
            {
                isOn = false,
                onActiveToggle = val => IsEditingTrapTarget = val
            }
        }).ToArray();
        List<UIDebugLevelEditorTrapToggleItem.Entity> mL = new List<UIDebugLevelEditorTrapToggleItem.Entity>();
        for (int i = 0; i < comp.Points.Count; i++)
        {
            int index = i;
            var ss = new UIDebugLevelEditorTrapToggleItem.Entity()
            {
                toggle = new UIComponentToggle.Entity()
                {
                    isOn = false,
                    onActiveToggle = val => OnChangeChildrenTarget(val, index)
                },
                title = $"Item_{index}",
            };
            mL.Add(ss);
        }
        return mL.ToArray();
    }

    private void OnChangeChildrenTarget(bool val, int index)
    {
        if (currentTrap == default) return;
        if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
        {
            if (val)
            {
                var comp = currentTrap.GetComponent<TrapEditorWoodSpike>();
                comp.SelectPoint(index);
            }
        }
    }

    private void OnAddChild()
    {
        if (currentTrap == default) return;
        if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
        {
            var comp = currentTrap.GetComponent<TrapEditorWoodSpike>();
            comp.AddNewPoint();
            uiDebugLevelEditor.trapEditor.SetChildren(new UIDebugLevelEditorTrapToggleList.Entity()
            {
                entities = GetListToggleWoodSpike(comp)
            });
        }
    }

    private void OnDeleteChild()
    {
        if (currentTrap == default) return;
        if (currentTrap.Type == TrapEditor.TYPE.WOODEN_SPIKE)
        {
            var comp = currentTrap.GetComponent<TrapEditorWoodSpike>();
            comp.DeleteSelectedPoint();
            uiDebugLevelEditor.trapEditor.SetChildren(new UIDebugLevelEditorTrapToggleList.Entity()
            {
                entities = GetListToggleWoodSpike(comp)
            });
        }
    }

}
#endif