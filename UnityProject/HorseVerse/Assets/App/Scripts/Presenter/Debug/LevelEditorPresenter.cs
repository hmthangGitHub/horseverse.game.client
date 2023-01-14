#if ENABLE_DEBUG_MODULE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public partial class LevelEditorPresenter : IDisposable
{
    private IDIContainer Container { get; }
    private CancellationTokenSource cts;
    
    private MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer;
    private MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer;
    private MasterHorseTrainingPropertyContainer masterHorseTrainingPropertyContainer;
    private MasterTrainingModularBlockContainer masterTrainingModularBlockContainer;
    
    private UIDebugLevelEditor uiDebugLevelEditor;
    private UIDebugLevelDesignBlockTransformPin uiDebugLevelDesignBlockTransformPinPrefab;
    private PlatformBase platformPrefab;
    private Material debugLineMaterial;
    private LevelEditorManager levelEditorManager;
    private Camera freeCameraComponent;

    private MasterTrainingBlockComboType CurrentBlockComboType
    {
        get => currentBlockComboType;
        set
        {
            if (currentBlockComboType == value) return;
            currentBlockComboType = value;
            OnSelectBlockComboType();
        }
    }


    private const string TrainingBlockSettingPath = "Maps/MapSettings/training_block_settings";
    private TrainingBlockSettings trainingBlockSettings;
    private MasterTrainingBlockComboType currentBlockComboType;

    public event Action OnBack = ActionUtility.EmptyAction.Instance;

    public LevelEditorPresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask ShowDebugMenuAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        await LoadAssetAsync();
        await SetEntityAsync();
    }

    private async UniTask LoadAssetAsync()
    {
        await LoadUIAssets();
        await LoadMasterAsync();
        await LoadInGameAssetAsync();
    }

    private async UniTask LoadUIAssets()
    {
        uiDebugLevelEditor ??= await UILoader.Instantiate<UIDebugLevelEditor>(UICanvas.UICanvasType.Debug, token: cts.Token);
        uiDebugLevelEditor.transform.SetAsFirstSibling();
        uiDebugLevelDesignBlockTransformPinPrefab =
            await UILoader.Instantiate<UIDebugLevelDesignBlockTransformPin>(token: cts.Token);
        uiDebugLevelDesignBlockTransformPinPrefab.gameObject.SetActive(false);
    }

    private UIDebugLevelDesignBlockTransformPin CreateUiPin(List<GameObject> storedList)
    {
        var uiDebugLevelDesignBlockTransformPin = Object.Instantiate(uiDebugLevelDesignBlockTransformPinPrefab, uiDebugLevelDesignBlockTransformPinPrefab.transform.parent);
        uiDebugLevelDesignBlockTransformPin.gameObject.SetActive(true);
        storedList.Add(uiDebugLevelDesignBlockTransformPin.gameObject);
        return uiDebugLevelDesignBlockTransformPin;
    }

    private void RemovePin(UIDebugLevelDesignBlockTransformPin pin)
    {
        Object.Destroy(pin.gameObject);
        blockComboPinList.Remove(pin.gameObject);
    }

    private async UniTask LoadMasterAsync()
    {
        (masterHorseTrainingBlockContainer,
        masterHorseTrainingBlockComboContainer,
        masterHorseTrainingPropertyContainer,
        masterTrainingModularBlockContainer,
        masterCoinPresetContainer) = await (MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockContainer>(cts.Token), 
                                                        MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockComboContainer>(cts.Token), 
                                                        MasterLoader.LoadMasterAsync<MasterHorseTrainingPropertyContainer>(cts.Token),
                                                        MasterLoader.LoadMasterAsync<MasterTrainingModularBlockContainer>(cts.Token),
                                                        MasterLoader.LoadMasterAsync<MasterCoinPresetContainer>(cts.Token));
        
        masterHorseTrainingProperty = masterHorseTrainingPropertyContainer.MasterHorseTrainingPropertyIndexer.First().Value;
    }

    private async UniTask LoadInGameAssetAsync()
    {
        var rootPrefab = await Resources.LoadAsync<LevelEditorManager>("GamePlay/Debug/LevelEditorManager") as LevelEditorManager;
        levelEditorManager = Object.Instantiate(rootPrefab);
        freeCameraComponent = levelEditorManager.GetComponentInChildren<Camera>(true);
        
        var horseTrainingManager = await Resources.LoadAsync<HorseTrainingManager>("GamePlay/HorseTrainingManager") as HorseTrainingManager;
        platformPrefab = horseTrainingManager.GetComponentInChildren<PlatformGeneratorModularBlock>()
                                             .platformPrefab;
        debugLineMaterial = await Resources.LoadAsync("GamePlay/Debug/debugLine") as Material;
        
        trainingBlockSettings = await PrimitiveAssetLoader.LoadAssetAsync<TrainingBlockSettings>(TrainingBlockSettingPath, cts.Token);
    }

    private async UniTask SetEntityAsync()
    {
        uiDebugLevelEditor.SetEntity(new UIDebugLevelEditor.Entity()
        {
            backBtn = new ButtonComponent.Entity(() => OnBack.Invoke()),
            editMode = UIDebugLevelEditorMode.Mode.None,
            saveBtn = new ButtonComponent.Entity(OnSave),
            editBlockToggle = new UIComponentToggle.Entity()
            {
                isOn = false,
                onActiveToggle = val => IsEditingBlock = val 
            },
            editObstacleToggle = new UIComponentToggle.Entity()
            {
                isOn = false,
                onActiveToggle = val => IsEditingObstacle = val
            },
            editCoinToggle = new UIComponentToggle.Entity()
            {
                isOn = false,
                onActiveToggle = val => IsEditingCoin = val
            },
            addObstacleBtn = new ButtonComponent.Entity(CreateNewObstacle),
            addCoinBtn = new ButtonComponent.Entity(CreateNewCoinEditor),
            blockComboType = new UIComponentBlockComboType.Entity()
            {
                defaultValue = UIComponentBlockComboType.BlockComboType.Modular,
                onValueChanged = val => CurrentBlockComboType = (MasterTrainingBlockComboType)(int)val
            },
            addFromPresetBtn = new ButtonComponent.Entity(() => AddCoinFromPresetAsync().Forget()),
        });
        await uiDebugLevelEditor.In();
        OnEditBlockComboBtn();
    }


    private void OnSave()
    {
        masterHorseTrainingBlockContainer.SaveToLocal();
        masterHorseTrainingBlockComboContainer.SaveToLocal();
        masterCoinPresetContainer.SaveToLocal();
    }

    private void UpdateEditMode(UIDebugLevelEditorMode.Mode editMode)
    {
        uiDebugLevelEditor.entity.editMode = editMode;
        uiDebugLevelEditor.editMode.SetEntity(uiDebugLevelEditor.entity.editMode);
    }

    private void AddMeshBoundaryDrawer(GameObject objGameObject)
    {
        var boxCollider = objGameObject.GetComponent<BoxCollider>();
        var color = Random.ColorHSV();
        MeshBoundaryDrawer.Instantiate(boxCollider, color, debugLineMaterial);
    }

    private async UniTask<bool> IsUserAgreeTo(string confirmText)
    {
        var ucs = new UniTaskCompletionSource<bool>();
        uiDebugLevelEditor.SetPopUpEntity(new UIDebugLevelEditorPopUp.Entity()
        {
            closeBtn = new ButtonComponent.Entity(() =>
            {
                uiDebugLevelEditor.entity.isPopUpVisible = false;
                uiDebugLevelEditor.isPopUpVisible.SetEntity(false);
                ucs.TrySetResult(false);
            }),
            confirmationText = confirmText,
            popUpMode = UIDebugLevelEditorPopUpMode.Mode.Confirmation,
            okBtn = new ButtonComponent.Entity(() =>
            {
                uiDebugLevelEditor.entity.isPopUpVisible = false;
                uiDebugLevelEditor.isPopUpVisible.SetEntity(false);
                ucs.TrySetResult(true);
            })
        });
        return await ucs.Task.AttachExternalCancellation(cts.Token);
    }
    
    private async UniTask<string> AskUserInput(string placeHolderText)
    {
        var ucs = new UniTaskCompletionSource<string>();
        uiDebugLevelEditor.SetPopUpEntity(new UIDebugLevelEditorPopUp.Entity()
        {
            closeBtn = new ButtonComponent.Entity(() =>
            {
                uiDebugLevelEditor.entity.isPopUpVisible = false;
                uiDebugLevelEditor.isPopUpVisible.SetEntity(false);
                ucs.TrySetResult(string.Empty);
            }),
            inputField = new UIComponentInputField.Entity()
            {
                placeHolderText = placeHolderText,
            },
            popUpMode = UIDebugLevelEditorPopUpMode.Mode.Confirmation,
            okBtn = new ButtonComponent.Entity(() =>
            {
                uiDebugLevelEditor.entity.isPopUpVisible = false;
                uiDebugLevelEditor.isPopUpVisible.SetEntity(false);
                ucs.TrySetResult(uiDebugLevelEditor.popUp.inputField.inputField.text);
            })
        });
        return await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        try
        {
            UnSelectOldBlockCombo();
            OnSave();
        }
        finally
        {
            cts.SafeCancelAndDispose();
            cts = default;
        
            MasterLoader.SafeRelease(ref masterHorseTrainingBlockContainer);
            MasterLoader.SafeRelease(ref masterHorseTrainingBlockComboContainer);
            MasterLoader.SafeRelease(ref masterHorseTrainingPropertyContainer);
            MasterLoader.SafeRelease(ref masterCoinPresetContainer);
        
            UILoader.SafeRelease(ref uiDebugLevelEditor);
            UILoader.SafeRelease(ref uiDebugLevelDesignBlockTransformPinPrefab);
        
            debugLineMaterial = default;
        
            blockComboPinList.ForEach(x => Object.Destroy(x.gameObject));
            blockComboPinList.Clear();
        
            DisposeUtility.SafeDispose(ref levelEditorManager);
            
            PrimitiveAssetLoader.UnloadAssetAtPath(TrainingBlockSettingPath);
            trainingBlockSettings = default;
        }
    }
}
#endif