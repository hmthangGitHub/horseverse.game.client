using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIMainMenuPresenter : IDisposable
{
    public UIMainMenu uiMainMenu;
    private CancellationTokenSource cts;

    public event Action OnBetModeBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnBreedingBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnInventoryBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnLibraryBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnPlayBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnStableBtn = ActionUtility.EmptyAction.Instance;
    public event Action OnTraningBtn = ActionUtility.EmptyAction.Instance;

    private HorseDetailEntityFactory horseDetailEntityFactory;
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactory ??= Container.Inject< HorseDetailEntityFactory>();
    private IUserDataRepository userDataRepository;
    private IUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IUserDataRepository>();
    private IDIContainer Container { get; }
    private IHorseRepository horseRepository;
    private IHorseRepository HorseRepository => horseRepository ??= Container.Inject<IHorseRepository>();

    private FeaturePresenter featurePresenter;
    public FeaturePresenter FeaturePresenter => featurePresenter ?? Container.Inject<FeaturePresenter>();
    public UIMainMenuPresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTaskVoid ShowMainMenuAsync()
    {
    	cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        await HorseRepository.LoadRepositoryIfNeedAsync();
        await UserDataRepository.LoadRepositoryIfNeedAsync();
        
        uiMainMenu ??= await UILoader.Instantiate<UIMainMenu>(token: cts.Token);
        uiMainMenu.SetEntity(new UIMainMenu.Entity()
        {
            betmodeBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnBetModeBtn).Forget(), FeaturePresenter.CheckFeature(FEATURE_TYPE.ARENA)),
            //breedingBtn = new ButtonComponent.Entity(OnBreedingBtn),
            //inventoryBtn = new ButtonComponent.Entity(OnInventoryBtn),
            //libraryBtn = new ButtonComponent.Entity(OnLibraryBtn),
            playBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnPlayBtn).Forget(), FeaturePresenter.CheckFeature(FEATURE_TYPE.RACING)),
            stableBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnStableBtn).Forget()),
            trainingBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnTraningBtn).Forget(), FeaturePresenter.CheckFeature(FEATURE_TYPE.ADVENTURE)),
            horseInfo = new UIComponentHorseBreedInfoAndDetail.Entity()
            {
                horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.CurrentHorseNftId),
            },
            
        });
        uiMainMenu.In().Forget();
    }

    private async UniTask TransitionToAsync(Action action)
    {
        await UniTask.Delay(200);
        await uiMainMenu.Out();
        action();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiMainMenu);
    }
}
