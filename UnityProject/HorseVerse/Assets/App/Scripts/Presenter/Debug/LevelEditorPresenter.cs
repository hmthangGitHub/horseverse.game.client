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
    private UIDebugLevelDesignBlockTransformPin[] blockSegmentPin;
    private UIDebugLevelDesignBlockTransformPin blockNamePin;
    private UIDebugLevelDesignBlockTransformPin uiDebugLevelDesignBlockTransformPinPrefab;
    private PlatformBase platformPrefab;
    private TrainingMapBlock trainingMapBlockPrefab;
    private GameObject freeCam;
    private Material debugLineMaterial;
    private GameObject root;
    private Camera freeCameraComponent;
    
    private const string TrainingBlockSettingPath = "Maps/MapSettings/training_block_settings";
    private TrainingBlockSettings trainingBlockSettings;

    public event Action OnBack = ActionUtility.EmptyAction.Instance;

    public LevelEditorPresenter(IDIContainer container)
    {
        Container = container;
        root = new GameObject("Root");
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
        await LoadBlockEditUIAssetsAsync();
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

    private UIDebugLevelDesignBlockTransformPin CreateUiPin()
    {
        var uiDebugLevelDesignBlockTransformPin = Object.Instantiate(uiDebugLevelDesignBlockTransformPinPrefab, uiDebugLevelDesignBlockTransformPinPrefab.transform.parent);
        uiDebugLevelDesignBlockTransformPin.gameObject.SetActive(true);
        blockComboPinList.Add(uiDebugLevelDesignBlockTransformPin.gameObject);
        return uiDebugLevelDesignBlockTransformPin;
    }

    private async UniTask LoadMasterAsync()
    {
        (masterHorseTrainingBlockContainer,
        masterHorseTrainingBlockComboContainer,
        masterHorseTrainingPropertyContainer,
        masterTrainingModularBlockContainer) = await (MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockContainer>(cts.Token), 
                                                        MasterLoader.LoadMasterAsync<MasterHorseTrainingBlockComboContainer>(cts.Token), 
                                                        MasterLoader.LoadMasterAsync<MasterHorseTrainingPropertyContainer>(cts.Token),
                                                        MasterLoader.LoadMasterAsync<MasterTrainingModularBlockContainer>(cts.Token));
        
        masterHorseTrainingProperty = masterHorseTrainingPropertyContainer.MasterHorseTrainingPropertyIndexer.First().Value;
    }

    private async UniTask LoadInGameAssetAsync()
    {
        var freeCamPrefab = await Resources.LoadAsync<GameObject>("GamePlay/Debug/LevelEditorFreeCamera") as GameObject;
        freeCam = Object.Instantiate(freeCamPrefab, root.transform);
        freeCameraComponent = freeCam.GetComponentInChildren<Camera>(true);
        
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
            editBlockBtn = new ButtonComponent.Entity(() =>
            {
                UnSelectOldBlockCombo();
                OnEditBlockBtn();
            }),
            editBlockComboBtn = new ButtonComponent.Entity(() =>
            {
                UnSelectOldBlock();
                OnEditBlockComboBtn();
            }),
            backBtn = new ButtonComponent.Entity(() => OnBack.Invoke()),
            editMode = UIDebugLevelEditorMode.Mode.None,
            saveBtn = new ButtonComponent.Entity(OnSave),
        });
        await uiDebugLevelEditor.In();
    }

    private void OnSave()
    {
        masterHorseTrainingBlockContainer.SaveToLocal();
        masterHorseTrainingBlockComboContainer.SaveToLocal();
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
            OnSave();
        }
        finally
        {
            cts.SafeCancelAndDispose();
            cts = default;
        
            MasterLoader.SafeRelease(ref masterHorseTrainingBlockContainer);
            MasterLoader.SafeRelease(ref masterHorseTrainingBlockComboContainer);
            MasterLoader.SafeRelease(ref masterHorseTrainingPropertyContainer);
        
            UILoader.SafeRelease(ref uiDebugLevelEditor);
            UILoader.SafeRelease(ref uiDebugLevelEditor);
            UILoader.SafeRelease(ref blockSegmentPin);
            UILoader.SafeRelease(ref blockNamePin);
            UILoader.SafeRelease(ref uiDebugLevelDesignBlockTransformPinPrefab);
        
            currentEditingTrainingMapBlockGameObject = default;
            debugLineMaterial = default;
        
            blockComboPinList.ForEach(x => Object.Destroy(x.gameObject));
            blockComboPinList.Clear();
        
            Object.Destroy(root);
            freeCameraComponent = default;
            
            PrimitiveAssetLoader.UnloadAssetAtPath(TrainingBlockSettingPath);
            trainingBlockSettings = default;
        }
    }
}
