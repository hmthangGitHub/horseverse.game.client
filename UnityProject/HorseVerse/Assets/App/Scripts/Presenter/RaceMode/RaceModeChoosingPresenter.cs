using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

internal class RaceModeChoosingPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIRacingMode uiRacingMode;
    private UITraditionalRoom uiTraditionalRoom;
    private CancellationTokenSource cts;
    private HorseRaceContext horseRaceContext;
    private UIHeaderPresenter uiHeaderPresenter;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= container.Inject<HorseRaceContext>();
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= container.Inject<UIHeaderPresenter>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    public event Action OnFinishChooseRaceMode = ActionUtility.EmptyAction.Instance;
    public event Action OnBack = ActionUtility.EmptyAction.Instance;
    public event Action OnViewHistory = ActionUtility.EmptyAction.Instance;
    
    public RaceModeChoosingPresenter(IDIContainer container)
    {
        this.container = container;
        UIHeaderPresenter.OnBack += OnHeaderBack;
    }

    private void OnHeaderBack()
    {
        OnBackBtnAsync().Forget();
    }

    private async UniTask OnBackBtnAsync()
    {
        if (HorseRaceContext.RaceMatchDataContext.RaceMode == RaceMode.None)
        {
            await uiRacingMode.Out();
            OnBack.Invoke();
        }
        else
        {
            await uiTraditionalRoom.Out();
            await uiRacingMode.In();
            HorseRaceContext.RaceMatchDataContext.RaceMode = RaceMode.None;
            HorseRaceContext.RaceMatchDataContext.RacingRoomType = RacingRoomType.None;
            await ChangeHeaderTitle();
        }
    }

    private string GetCurrentTitle()
    {
        return HorseRaceContext.RaceMatchDataContext.RacingRoomType == RacingRoomType.None 
            ? HorseRaceContext.RaceMatchDataContext.RaceMode == RaceMode.None 
            ? GetRaceModeTitle()
            : GetMasteryRoomTitle()
            : string.Empty;
    }

    private static string GetRaceModeTitle()
    {
        return "RACE";
    }

    private static string GetMasteryRoomTitle()
    {
        return "TRADITIONAL";
    }

    public async UniTaskVoid ChooseRaceModeAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        await UITouchDisablePresenter.ShowTillFinishTaskAsync(InstantiateUIAsync());

        uiRacingMode.SetEntity(new UIRacingMode.Entity()
        {
            isRankLock = true,
            isTournamentLock = true,
            isStableVsStableLock = true,
            isTraditionalLock = false,
            historyBtn = new ButtonComponent.Entity(UniTask.Action(async() =>
            {
                await uiRacingMode.Out();
                OnViewHistory.Invoke();
            }), false),
            traditionalBtn = SelectRaceModeBtnEntity(RaceMode.Traditional),
            rankBtn = SelectRaceModeBtnEntity(RaceMode.Rank),
            tournamentBtn = SelectRaceModeBtnEntity(RaceMode.Tournament),
            stableVsStableBtn = SelectRaceModeBtnEntity(RaceMode.StableVsStable),
        });
        uiRacingMode.historyBtn.gameObject.SetActive(false);


        if (HorseRaceContext.RaceMatchDataContext.RaceMode == RaceMode.None)
        {
            await ChangeHeaderTitle();
            await uiRacingMode.In();
        }
        else
        {
            await SelectRaceModeAsync();
        }
    }

    private async UniTask InstantiateUIAsync()
    {
        uiRacingMode = await UILoader.Instantiate<UIRacingMode>(token: cts.Token);
        uiTraditionalRoom = await UILoader.Instantiate<UITraditionalRoom>(token: cts.Token);
    }

    private async UniTask ChangeHeaderTitle()
    {
        await UIHeaderPresenter.ShowHeaderAsync(true, GetCurrentTitle());
    }

    private ButtonComponent.Entity SelectRaceModeBtnEntity(RaceMode raceMode)
    {
        return new ButtonComponent.Entity(UniTask.Action(async () =>
        {
            await uiRacingMode.Out();
            HorseRaceContext.RaceMatchDataContext.RaceMode = raceMode;
            await SelectRaceModeAsync();
        }));
    }

    private async UniTask SelectRaceModeAsync()
    {
        await ChangeHeaderTitle();
        SelectMasteryTypeForRaceMode(HorseRaceContext.RaceMatchDataContext.RaceMode);
        await uiTraditionalRoom.In();
    }

    private void SelectMasteryTypeForRaceMode(RaceMode raceMode)
    {
        switch (raceMode)
        {
            case RaceMode.Traditional:
                uiTraditionalRoom.SetEntity(new UITraditionalRoom.Entity()
                {
                    noviceRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(RacingRoomType.Novice),
                        isLock = false
                    },
                    advanceRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(RacingRoomType.Advance),
                        isLock = true
                    },
                    basicRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(RacingRoomType.Basic),
                        isLock = true
                    },
                    expertRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(RacingRoomType.Expert),
                        isLock = true
                    },
                    masterRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(RacingRoomType.Master),
                        isLock = true
                    },
                });
                break;
            case RaceMode.StableVsStable:
            case RaceMode.Rank:
            case RaceMode.Tournament:
            default:
                throw new ArgumentOutOfRangeException(nameof(raceMode), raceMode, null);
        }
    }

    private ButtonComponent.Entity SelectMasteryBtnEntity(RacingRoomType horseRarity)
    {
        return new ButtonComponent.Entity(UniTask.Action(async () =>
        {
            HorseRaceContext.RaceMatchDataContext.RacingRoomType = horseRarity;
            await uiTraditionalRoom.Out();
            await ChangeHeaderTitle();
            OnFinishChooseRaceMode.Invoke();
        }));
    }

    public void Dispose()
    {
        UIHeaderPresenter.OnBack -= OnHeaderBack;
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiRacingMode);
        UILoader.SafeRelease(ref uiTraditionalRoom);
    }
}