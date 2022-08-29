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
    private IDIContainer container = default;
    public event Action OnBack = ActionUtility.EmptyAction.Instance;
    public event Action OnToRaceMode = ActionUtility.EmptyAction.Instance;
    private IBetRateRepository betRateRepository = default;
    public IBetRateRepository BetRateRepository => betRateRepository ??= container.Inject<IBetRateRepository>();

    private IBetModeDomainService betModeDomainService = default;
    private IBetModeDomainService BetModeDomainService => betModeDomainService ??= container.Inject<IBetModeDomainService>();

    private IReadOnlyUserDataRepository userDataRepository = default;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();

    private IReadOnlyBetMatchRepository betMatchRepository = default;
    private IReadOnlyBetMatchRepository BetMatchRepository => betMatchRepository ??= container.Inject<IReadOnlyBetMatchRepository>();

    private int currentBettingAmouth = 0;

    public UIBetModePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIBetModeAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        await BetRateRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        await BetMatchRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        BetRateRepository.OnModelUpdate += BetRateRepositoryOnModelUpdate;
        BetRateRepository.OnModelsUpdate += BetRateRepositoryOnModelsUpdate;
        
        uiBetMode ??= await UILoader.Instantiate<UIBetMode>(token: cts.Token);
        uiBetConfirmation ??= await UILoader.Instantiate<UIBetConfirmation>(token: cts.Token);
        
        uiBetMode.SetEntity(new UIBetMode.Entity()
        {
            backBtn = new ButtonComponent.Entity(() => TransitionAsync(OnBack).Forget()),
            betAmouthsContainer = new UIComponentBetAmouthsContainer.Entity()
            {
                betAmouthIndicator = new UIComponentBetAmouthIndicator.Entity()
                {
                    betAmouthListScroller = new UIComponentBetAmouthListScroller.Entity()
                    {
                        entities = GetBetAmouthEntities()
                    },
                    OnFocusIndex = OnSelectBetAmouthAtIndex
                },
                cancelBtn = new ButtonComponent.Entity(() => BetModeDomainService.CancelBetAsync().Forget()),
                totalBetAmouth = (int)BetRateRepository.TotalBetAmouth,
            },
            singleBetSlotList = new UIComponentSingleBetSlotList.Entity()
            {
                entities = BetRateRepository.Models.Where(x => x.Key.second == default)
                .Select(x => new UIComponentBetSlot.Entity()
                {
                    horseNumber = x.Key.first,
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
                    firstHorseNumber = x.Key.first,
                    secondHorseNumber = x.Key.second,
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
                },
                userInfo = new UIComponentBetModeUserInfo.Entity()
                {
                    coin = userDataRepository.Current.Coin,
                    userName = userDataRepository.Current.UserName,
                }
            },
            quickBetButtonsContainer = new UIComponentQuickBetButtonsContainer.Entity()
            {
                onBetAll = val => OnBetAllAtHorseNumber(val).Forget()
            }
        });

        await uiBetMode.In();
    }

    private async UniTaskVoid OnChangeToRaceModeAsync()
    {
        var raceMatchData = await BetModeDomainService.GetCurrentBetModeRaceMatchData();
        container.Bind(raceMatchData);
        TransitionAsync(OnToRaceMode).Forget();
    }

    private async UniTaskVoid OnBetAllAtHorseNumber(int horseNumber)
    {
        if (await AskIsConfirmBet())
        {
            var keys = BetRateRepository.Models.Where(x => x.Key.second != default && x.Key.first == horseNumber || x.Key.second == horseNumber)
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

    private UIComponentBetAmouth.Entity[] GetBetAmouthEntities()
    {
#if MOCK_DATA
        return new UIComponentBetAmouth.Entity[]
        {
            new UIComponentBetAmouth.Entity()
            {
                betAmouth = 1000,
            },
            new UIComponentBetAmouth.Entity()
            {
                betAmouth = 2000,
            },
            new UIComponentBetAmouth.Entity()
            {
                betAmouth = 3000,
            },
            new UIComponentBetAmouth.Entity()
            {
                betAmouth = 4000,
            },
            new UIComponentBetAmouth.Entity()
            {
                betAmouth = 5000,
            },
            new UIComponentBetAmouth.Entity()
            {
                betAmouth = 6000,
            }
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
        currentBettingAmouth = uiBetMode.betAmouthsContainer.betAmouthIndicator
                                                            .betAmouthListScroller
                                                            .entity.entities[index]
                                                            .betAmouth;
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
    }
}
