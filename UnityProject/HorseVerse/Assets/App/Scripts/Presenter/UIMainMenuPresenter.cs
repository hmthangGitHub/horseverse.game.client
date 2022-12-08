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
            betmodeBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnBetModeBtn).Forget()),
            //breedingBtn = new ButtonComponent.Entity(OnBreedingBtn),
            //inventoryBtn = new ButtonComponent.Entity(OnInventoryBtn),
            //libraryBtn = new ButtonComponent.Entity(OnLibraryBtn),
            playBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnPlayBtn).Forget()),
            stableBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnStableBtn).Forget()),
            trainingBtn = new ButtonComponent.Entity(() => TransitionToAsync(OnTraningBtn).Forget()),
            horseInfo = new UIComponentHorseBreedInfoAndDetail.Entity()
            {
                //horseBreedProgressList = new UIComponentHorseBreedProgressList.Entity()
                //{
                //    entities = new UIComponentHorseBreedProgressType.Entity[] 
                //    {
                //        new UIComponentHorseBreedProgressType.Entity()
                //        {
                //            breedType = UIComponentHorseBreedProgressType.BreedType.Night,
                //            progress = 0.25f
                //        },
                //        new UIComponentHorseBreedProgressType.Entity()
                //        {
                //            breedType = UIComponentHorseBreedProgressType.BreedType.Thunder,
                //            progress = 0.35f
                //        },
                //        new UIComponentHorseBreedProgressType.Entity()
                //        {
                //            breedType = UIComponentHorseBreedProgressType.BreedType.Light,
                //            progress = 0.65f
                //        }
                //    }
                //},
                horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.CurrentHorseNftId),
            },
            //userInfo = new UIComponentMainMenuUserInfo.Entity()
            //{
            //    energy = UserDataRepository.Current.Energy,
            //    energyMax = UserDataRepository.Current.MaxEnergy,
            //    level = UserDataRepository.Current.Level,
            //    levelIcon = string.Empty,
            //    levelProgressBar = new UIComponentProgressBar.Entity()
            //    {
            //        progress = (float)UserDataRepository.Current.Exp / UserDataRepository.Current.NextLevelExp
            //    },
            //    currentExp = UserDataRepository.Current.Exp,
            //    maxExp = UserDataRepository.Current.NextLevelExp,
            //    userName = UserDataRepository.Current.UserName
            //}
            horseRace = new UIComponentHorseRace.Entity()
            {
                type = HorseDetailEntityFactory.GetHorseRace(UserDataRepository.Current.CurrentHorseNftId)
            }
        });
        uiMainMenu.In().Forget();
    }

    private async UniTask TransitionToAsync(Action action)
    {
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
