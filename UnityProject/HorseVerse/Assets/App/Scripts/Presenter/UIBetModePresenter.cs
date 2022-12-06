#define MOCK_DATA
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIBetModePresenter : IDisposable
{
    private const int HorseNumber = 8;
    private UIBetMode uiBetMode = default;
    private UIBetConfirmation uiBetConfirmation = default;
    private CancellationTokenSource cts = default;
    private readonly IDIContainer container = default;
    
    public event Action OnBack = ActionUtility.EmptyAction.Instance;
    public event Action OnToRaceMode = ActionUtility.EmptyAction.Instance;
    public event Action OnTimeOut = ActionUtility.EmptyAction.Instance;
    
    private IBetRateRepository betRateRepository = default;
    private IBetModeDomainService betModeDomainService = default;
    private IReadOnlyUserDataRepository userDataRepository = default;
    private IReadOnlyBetMatchRepository betMatchRepository = default;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    
    public IBetRateRepository BetRateRepository => betRateRepository ??= container.Inject<IBetRateRepository>();
    private IBetModeDomainService BetModeDomainService => betModeDomainService ??= container.Inject<IBetModeDomainService>();
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyBetMatchRepository BetMatchRepository => betMatchRepository ??= container.Inject<IReadOnlyBetMatchRepository>();
    private UIHorse3DViewPresenter UiHorse3DViewPresenter => uiHorse3DViewPresenter ??= container.Inject<UIHorse3DViewPresenter>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();

    private int currentBettingAmouth = 0;

    public UIBetModePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIBetModeAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        BetRateRepository.OnModelUpdate += BetRateRepositoryOnModelUpdate;
        BetRateRepository.OnModelsUpdate += BetRateRepositoryOnModelsUpdate;
        UserDataRepository.OnModelUpdate += OnModelUpdate;
        
        uiBetMode ??= await UILoader.Instantiate<UIBetMode>(token: cts.Token);
        uiBetConfirmation ??= await UILoader.Instantiate<UIBetConfirmation>(token: cts.Token);
        
        uiBetMode.SetEntity(new UIBetMode.Entity()
        {
            betAmouthsContainer = new UIComponentBetAmouthsContainer.Entity()
            {
                cancelBtn = new ButtonComponent.Entity(() => BetModeDomainService.CancelBetAsync().Forget()),
                totalBetAmouth = (int)BetRateRepository.TotalBetAmouth,
                betAmounths = new UIComponentBetAmouthList.Entity()
                {
                    betAmouthList = GetBetAmouthEntities(),
                    OnFocusIndex = OnSelectBetAmouthAtIndex
                },
            },
            singleBetSlotList = new UIComponentSingleBetSlotList.Entity()
            {
                entities = BetRateRepository.Models.Where(x => x.Key.second == default)
                .Select(x => new UIComponentBetSlot.Entity()
                {
                    horseNumber = new UIComponentBetSlotNumber.Entity() { Number = x.Key.first },
                    betRatio = x.Value.Rate,
                    betType = UIComponentBetSlotType.BetType.SingleBet,
                    totalBet = x.Value.TotalBet,
                    betBtn = new ButtonComponent.Entity(() => OnBetAtSlotAsync(x.Key).Forget()),
                }).ToArray()
            },
            doubleBetSlotList = new UIComponentDoubleBetList.Entity()
            {
                entities = BetRateRepository.Models.Where(x => x.Key.second != default)
                .Select(x => new UIComponentBetSlot.Entity()
                {
                    betRatio = x.Value.Rate,
                    betType = UIComponentBetSlotType.BetType.DoubleBet,
                    firstHorseNumber = new UIComponentBetSlotNumber.Entity() { Number = x.Key.first },
                    secondHorseNumber = new UIComponentBetSlotNumber.Entity() { Number = x.Key.second },
                    totalBet = x.Value.TotalBet,
                    betBtn = new ButtonComponent.Entity(() => OnBetAtSlotAsync(x.Key).Forget()),
                }).ToArray()
            },
            header = new UIComponentBetModeHeader.Entity()
            {
                changeRaceBtn = new ButtonComponent.Entity(ChangeRaceBtn),
                energy = UserDataRepository.Current.Energy,
                maxEnergy = UserDataRepository.Current.MaxEnergy,
                timeCountDown = new UIComponentCountDownTimer.Entity()
                {
                    outDatedEvent = () => OnChangeToRaceModeAsync().Forget(),
                    utcEndTimeStamp = (int)BetMatchRepository.Current.BetMatchTimeStamp
                    // utcEndTimeStamp = 1671611174
                },
                header = new UIHeader.Entity()
                {
                    coin = userDataRepository.Current.Coin,
                    userName = userDataRepository.Current.UserName,
                    energy = UserDataRepository.Current.Energy,
                    maxEnergy = UserDataRepository.Current.MaxEnergy,
                    backBtn = new ButtonComponent.Entity(() => TransitionAsync(OnBack).Forget()),
                }
            },
            quickBetButtonsContainer = new UIComponentQuickBetButtonsContainer.Entity()
            {
                onBetAll = val => OnBetAllAtHorseNumber(val).Forget()
            }
        });

        if (uiBetMode)
        {
            uiBetMode.header.header.SetVisibleBackBtn(true);
            uiBetMode.header.header.SetTitle("BETTING");
            uiBetMode.header.header.In().Forget();
            await uiBetMode.In();
        }
    }

    private void OnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.after.Coin != model.before?.Coin)
        {
            uiBetMode?.header.header.coin.SetEntity(model.after.Coin);
        }
    }

    private async UniTaskVoid OnChangeToRaceModeAsync()
    {
        if (BetRateRepository.Models.Any(x => x.Value.TotalBet > 0))
        {
            await UITouchDisablePresenter.Delay(1.5f);
            var raceMatchData = await BetModeDomainService.GetCurrentBetModeRaceMatchData();
            container.Bind(raceMatchData);
            UiHorse3DViewPresenter.Dispose();
            Debug.Log("X1");
            TransitionAsync(OnToRaceMode).Forget();    
        }
        else
        {
            await UITouchDisablePresenter.Delay(1.5f);
            Debug.Log("X2");
            TransitionAsync(OnTimeOut).Forget();
        }
    }

    private async UniTaskVoid OnBetAllAtHorseNumber(int horseNumber)
    {
        if (await AskIsConfirmBet())
        {
            var keys = BetRateRepository.Models.Where(x => (x.Key.second != default && x.Key.first == horseNumber || x.Key.second == horseNumber) 
                                                           && (x.Key.second > x.Key.first))
                .Select(x => x.Key)
                .ToArray();
            BetModeDomainService.BetAsync(keys, currentBettingAmouth).Forget();    
        }
    }

    private void BetRateRepositoryOnModelsUpdate((IReadOnlyDictionary<(int first, int second), BetRateModel> before, IReadOnlyDictionary<(int first, int second), BetRateModel> after) obj)
    {
        uiBetMode.betAmouthsContainer.SetTotalBetAmouth((int)BetRateRepository.TotalBetAmouth);
    }

    private void BetRateRepositoryOnModelUpdate((BetRateModel before, BetRateModel after) model)
    {
        if (model.after.Second == default)
        {
            uiBetMode.singleBetSlotList.instanceList[model.after.First - 1].SetTotalBetAmouth((int)model.after.TotalBet);
        }
        else
        {
            int betSlotIndex = (model.after.First - 1) * HorseNumber + (model.after.Second - 1);
            uiBetMode.doubleBetSlotList.instanceList[betSlotIndex].SetTotalBetAmouth((int)model.after.TotalBet);
        }
    }

    private void ChangeRaceBtn()
    {
        //TODO
    }

    private async UniTaskVoid OnBetAtSlotAsync((int first, int second) key)
    {
        var confirm = await AskIsConfirmBet();
        if (confirm)
        {
            BetModeDomainService.BetAsync(new (int first, int second)[] { key }, currentBettingAmouth).Forget();    
        }
    }

    private async UniTask<bool> AskIsConfirmBet()
    {
        if (UserSettingLocalRepository.IsSkipConfirmBet)
        {
            return true;
        }
        else
        {
            var ucs = new UniTaskCompletionSource<bool>();
            uiBetConfirmation.SetEntity(new UIBetConfirmation.Entity()
            {
                cancelBtn = new ButtonComponent.Entity(() =>
                {
                    ucs.TrySetResult(false);
                }),
                confirmBtn = new ButtonComponent.Entity(() =>
                {
                    ucs.TrySetResult(true);
                }),
                showAgain = new UIComponentToggle.Entity()
                {
                    isOn = UserSettingLocalRepository.IsSkipConfirmBet,
                    onActiveToggle = val => UserSettingLocalRepository.IsSkipConfirmBet = val
                }
            });
            await uiBetConfirmation.In();
            await ucs.Task;
            await uiBetConfirmation.Out();
            return ucs.Task.GetAwaiter().GetResult();
        }
    }

    private int[] GetBetAmouthEntities()
    {
#if MOCK_DATA
        return new int[]
        {
            10,20,30,40,50,60
        };
#endif
    }

    private async UniTaskVoid TransitionAsync(Action action)
    {
        await uiBetMode.Out();
        action();
    }

    private void OnSelectBetAmouthAtIndex(int index)
    {
#if MOCK_DATA
        currentBettingAmouth = uiBetMode.betAmouthsContainer.betAmounths
                                                            .entity.betAmouthList[index];
#else
        throw new NotImplementedException();
#endif
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiBetMode);
        UILoader.SafeRelease(ref uiBetConfirmation);
        BetRateRepository.OnModelUpdate -= BetRateRepositoryOnModelUpdate;
        BetRateRepository.OnModelsUpdate -= BetRateRepositoryOnModelsUpdate;

        userDataRepository = default;
        betRateRepository = default;
        betMatchRepository = default;
        betModeDomainService = default;
        currentBettingAmouth = default;
        uiHorse3DViewPresenter = default;
    }
}
