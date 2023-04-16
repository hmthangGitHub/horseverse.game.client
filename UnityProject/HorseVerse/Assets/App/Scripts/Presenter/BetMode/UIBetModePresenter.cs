using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    private CancellationTokenSource askForConfirmCts = default;
    private readonly IDIContainer container = default;
    private UIBetModeHorseInfo uiBetModeHorseInfo = default;
    private IBetRateRepository betRateRepository = default;
    private IBetModeDomainService betModeDomainService = default;
    private IReadOnlyUserDataRepository userDataRepository = default;
    private IReadOnlyBetMatchRepository betMatchRepository = default;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIHorseInfo3DViewPresenter uiHorseInfo3DViewPresenter;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private MasterHorseContainer masterHorseContainer;
    private HorseRaceContext horseRaceContext;
    private UIHeaderPresenter uiHeaderPresenter;
    private HorseBetInfo horseBetInfo = default;
    private int currentBettingAmouth = 0;
    private int currentHorseInfoView = -1;
    private CancellationTokenSource ctsInfo;
    private const int BetModeBufferTime = 2;

    private IBetRateRepository BetRateRepository => betRateRepository ??= container.Inject<IBetRateRepository>();
    private IBetModeDomainService BetModeDomainService => betModeDomainService ??= container.Inject<IBetModeDomainService>();
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyBetMatchRepository BetMatchRepository => betMatchRepository ??= container.Inject<IReadOnlyBetMatchRepository>();
    private UIHorse3DViewPresenter UiHorse3DViewPresenter => uiHorse3DViewPresenter ??= container.Inject<UIHorse3DViewPresenter>();
    private UIHorseInfo3DViewPresenter UiHorseInfo3DViewPresenter => uiHorseInfo3DViewPresenter ??= container.Inject<UIHorseInfo3DViewPresenter>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= container.Inject<HorseRaceContext>();
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= container.Inject<UIHeaderPresenter>();

    private int CurrentBettingAmouth
    {
        get => currentBettingAmouth;
        set
        {
            if (currentBettingAmouth == value) return;
            currentBettingAmouth = value;
            OnUpdateBettingButtons();
        }
    }
    
    public event Action OnViewHistory = ActionUtility.EmptyAction.Instance;
    public event Action OnBack = ActionUtility.EmptyAction.Instance;
    public event Action OnToRaceMode = ActionUtility.EmptyAction.Instance;
    public event Action OnTimeOut = ActionUtility.EmptyAction.Instance;

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
        uiBetModeHorseInfo ??= await UILoader.Instantiate<UIBetModeHorseInfo>(token: cts.Token);

        horseBetInfo = await BetModeDomainService.GetCurrentBetModeHorseData();

        uiBetMode.SetEntity(new UIBetMode.Entity()
        {
            betAmouthsContainer = new UIComponentBetAmouthsContainer.Entity()
            {
                cancelBtn = new ButtonComponent.Entity(() =>
                {
                    if (BetRateRepository.TotalBetAmouth > 0)
                    {
                        BetModeDomainService.CancelBetAsync().Forget();
                    }
                }),
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
                timeCountDown = new UIComponentCountDownTimer.Entity()
                {
                    outDatedEvent = () =>
                    {
                        askForConfirmCts.SafeCancelAndDispose();
                        OnChangeToRaceModeAsync()
                            .Forget();
                    },
                    utcEndTimeStamp = (int)BetMatchRepository.Current.BetMatchTimeStamp - BetModeBufferTime,
                },
            },
            quickBetButtonsContainer = new UIComponentQuickBetButtonsContainer.Entity()
            {
                onBetAll = val => OnBetAllAtHorseNumber(val).Forget()
            },
            btnHorseList = new ButtonComponent.Entity(()=> OpenHorseList().Forget()),
            historyBtn = new ButtonComponent.Entity(OnViewHistory)
        });

        await uiBetMode.In();

        SoundController.PlayMusicBetModePrepare();
        OnUpdateBettingButtons();
    }

    private void OnUpdateBettingButtons()
    {
        uiBetMode.singleBetSlotList.instanceList.ForEach(x => x.betBtn.SetInteractable(UserDataRepository.Current.Coin >= CurrentBettingAmouth));
        uiBetMode.doubleBetSlotList.instanceList.ForEach(x => x.betBtn.SetInteractable(UserDataRepository.Current.Coin >= CurrentBettingAmouth));
        uiBetMode.quickBetButtonsContainer.SetInteractable(UserDataRepository.Current.Coin >= CurrentBettingAmouth * 7);
    }

    private void OnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.after.Coin != model.before?.Coin)
        {
            OnUpdateBettingButtons();
        }
    }

    private async UniTaskVoid OnChangeToRaceModeAsync()
    {
        await UITouchDisablePresenter.Delay(BetModeBufferTime + 1.5f);
        if (BetRateRepository.Models.Any(x => x.Value.TotalBet > 0))
        {
            var betMatchData = await BetModeDomainService.GetCurrentBetMatchData();

            HorseRaceContext.RaceMatchData = betMatchData.raceScriptData;
            HorseRaceContext.HorseBriefInfos = betMatchData.raceScriptData.HorseRaceInfos;
            HorseRaceContext.BetMatchDataContext = betMatchData.betMatchDataContext;
            HorseRaceContext.MasterMapId = RacingState.MasterMapId;

            UiHorse3DViewPresenter.Dispose();
            UiHorseInfo3DViewPresenter.Dispose();
            TransitionAsync(OnToRaceMode).Forget();    
        }
        else
        {
            UiHorseInfo3DViewPresenter.Dispose();
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
            BetModeDomainService.BetAsync(keys, CurrentBettingAmouth).Forget();    
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
            BetModeDomainService.BetAsync(new (int first, int second)[] { key }, CurrentBettingAmouth).Forget();    
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
            askForConfirmCts.SafeCancelAndDispose();
            askForConfirmCts = new CancellationTokenSource();
            try
            {
                var ucs = new UniTaskCompletionSource<bool>();
                uiBetConfirmation.SetEntity(new UIBetConfirmation.Entity()
                {
                    cancelBtn = new ButtonComponent.Entity(() => { ucs.TrySetResult(false); }),
                    confirmBtn = new ButtonComponent.Entity(() => { ucs.TrySetResult(true); }),
                    showAgain = new UIComponentToggle.Entity()
                    {
                        isOn = UserSettingLocalRepository.IsSkipConfirmBet,
                        onActiveToggle = val => UserSettingLocalRepository.IsSkipConfirmBet = val
                    }
                });
                await uiBetConfirmation.In()
                                       .AttachExternalCancellation(askForConfirmCts.Token);
                await ucs.Task.AttachExternalCancellation(askForConfirmCts.Token);
                return ucs.Task.GetAwaiter().GetResult();
            }
            finally
            {
                await uiBetConfirmation.Out().AttachExternalCancellation(cts.Token);
            }
        }
    }

    private int[] GetBetAmouthEntities()
    {
        return UserSettingLocalRepository.MasterDataModel.BetNumberList.ToArray();
    }

    private async UniTaskVoid TransitionAsync(Action action)
    {
        SoundController.PlayMusicBase();
        UIHeaderPresenter.HideHeader();
        await uiBetMode.Out();
        action();
    }

    private void OnSelectBetAmouthAtIndex(int index)
    {
        CurrentBettingAmouth = uiBetMode.betAmouthsContainer.betAmounths
                                                            .entity.betAmouthList[index];
    }

    private async UniTask OpenHorseList()
    {
        ctsInfo.SafeCancelAndDispose();
        ctsInfo = new CancellationTokenSource();
        var ucs = new UniTaskCompletionSource();
        uiBetModeHorseInfo.SetEntity(new UIBetModeHorseInfo.Entity()
        {
            backBtn = new ButtonComponent.Entity(()=>
            {
                UiHorseInfo3DViewPresenter.HideHorse3DViewAsync().Forget();
                ucs.TrySetResult();
            }),
            horseList = new UIComponentBetModeHorseInfoList.Entity() { 
                entities = GetHorseInfo()
            },
            horseDetail = new UIComponentHorseDetail.Entity(),
            horseDetailNumber = 1,
            horseRace = new UIComponentHorseRace.Entity(),
        });
        await UITouchDisablePresenter.Delay(1.5f, ctsInfo.Token);
        await OnUpdateHorseInfoView(0);
        await uiBetModeHorseInfo.In();
        await ucs.Task.AttachExternalCancellation(cts.Token);
        await uiBetModeHorseInfo.Out();
    }

    private UIComponentBetModeHorseInfoItem.Entity[] GetHorseInfo()
    {
        if(horseBetInfo != default && horseBetInfo.HorseInfos != default)
        {
            int len = horseBetInfo.HorseInfos.Length;
            var data = new List<UIComponentBetModeHorseInfoItem.Entity>();
            for(int i = 0; i < len; i++)
            {
                int index = i;
                // var userHorse = 
                var item = new UIComponentBetModeHorseInfoItem.Entity()
                {
                    no = i + 1,
                    horseName = horseBetInfo.HorseInfos[i].Name,
                    avgRec = horseBetInfo.HorseInfos[i].AverageBettingRecord,
                    bestRec = horseBetInfo.HorseInfos[i].BestBettingRecord,
                    lastMatch = horseBetInfo.HorseInfos[i].LastBettingRecord,
                    rate = BetRateRepository.Models[(i + 1, default)].Rate,
                    button = new ButtonSelectedComponent.Entity(()=> { OnUpdateHorseInfoView(index).Forget();}, index == 0)
                };
                data.Add(item);
            }
            return data.ToArray();
        }
        return null;
    }

    private async UniTask OnUpdateHorseInfoView(int index)
    {
        bool update = false;
        if (currentHorseInfoView > -1)
        {
            var e = uiBetModeHorseInfo.horseList.instanceList[currentHorseInfoView];
            e.button.SetSelected(false);
            currentHorseInfoView = -1;
            update = true;
        }

        if (index < uiBetModeHorseInfo.horseList.instanceList.Count)
        {
            var curr = uiBetModeHorseInfo.horseList.instanceList[index];
            curr.button.SetSelected(true);
            currentHorseInfoView = index;
            var horseInfo = horseBetInfo.HorseInfos[index];
            var entity = uiBetModeHorseInfo.entity;
            entity.horseDetail = new UIComponentHorseDetail.Entity()
            {
                horseName = horseInfo.Name,
                powerProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = horseInfo.PowerBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = horseInfo.PowerRatio
                    }
                },
                speedProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = horseInfo.SpeedBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = horseInfo.SpeedRatio
                    }
                },
                technicallyProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
                {
                    bonus = horseInfo.TechnicallyBonus,
                    progressBar = new UIComponentProgressBar.Entity()
                    {
                        progress = horseInfo.TechnicallyRatio
                    }
                },
            };
            entity.horseDetailNumber = index + 1;
            entity.horseRace = new UIComponentHorseRace.Entity()
            {
                type = (int)horseInfo.HorseType
            };
            
            uiBetModeHorseInfo.UpdateDetailInfo(entity);
            if (!update)
                await UiHorseInfo3DViewPresenter.ShowHorse3DViewAsync(MasterHorseContainer.FromTypeToMasterHorse(horseInfo.HorseType).MasterHorseId, horseInfo.Color1, horseInfo.Color2, horseInfo.Color3, horseInfo.Color4).AttachExternalCancellation(ctsInfo.Token);
            else
                await UiHorseInfo3DViewPresenter.UpdateMode(MasterHorseContainer.FromTypeToMasterHorse(horseInfo.HorseType).MasterHorseId, horseInfo.Color1, horseInfo.Color2, horseInfo.Color3, horseInfo.Color4).AttachExternalCancellation(ctsInfo.Token);
        }
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref askForConfirmCts);
        DisposeUtility.SafeDispose(ref ctsInfo);
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDispose(ref uiHorseInfo3DViewPresenter);
        
        UILoader.SafeRelease(ref uiBetMode);
        UILoader.SafeRelease(ref uiBetConfirmation);
        UILoader.SafeRelease(ref uiBetModeHorseInfo);

        BetRateRepository.OnModelUpdate -= BetRateRepositoryOnModelUpdate;
        BetRateRepository.OnModelsUpdate -= BetRateRepositoryOnModelsUpdate;
        UserDataRepository.OnModelUpdate -= OnModelUpdate;

        userDataRepository = default;
        betRateRepository = default;
        betMatchRepository = default;
        betModeDomainService = default;
        currentBettingAmouth = default;
        uiHorse3DViewPresenter = default;
        uiHorseInfo3DViewPresenter = default;
        horseRaceContext = default;
        uiHeaderPresenter = default;
    }
}
