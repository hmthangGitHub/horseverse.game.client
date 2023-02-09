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
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= container.Inject<HorseRaceContext>();
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= container.Inject<UIHeaderPresenter>();
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
            HorseRaceContext.RaceMatchDataContext.TraditionalRoomMasteryType = TraditionalRoomMasteryType.None;
        }
        await ChangeHeaderTitle();
    }

    private string GetCurrentTitle()
    {
        return HorseRaceContext.RaceMatchDataContext.TraditionalRoomMasteryType == TraditionalRoomMasteryType.None 
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
        uiRacingMode = await UILoader.Instantiate<UIRacingMode>(token: cts.Token);
        uiTraditionalRoom = await UILoader.Instantiate<UITraditionalRoom>(token: cts.Token);
        
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
            })),
            traditionalBtn = SelectRaceModeBtnEntity(RaceMode.Traditional),
            rankBtn = SelectRaceModeBtnEntity(RaceMode.Rank),
            tournamentBtn = SelectRaceModeBtnEntity(RaceMode.Tournament),
            stableVsStableBtn = SelectRaceModeBtnEntity(RaceMode.StableVsStable),
        });
        
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
                        btn = SelectMasteryBtnEntity(TraditionalRoomMasteryType.Novice),
                        isLock = false
                    },
                    advanceRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(TraditionalRoomMasteryType.Advance),
                        isLock = true
                    },
                    basicRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(TraditionalRoomMasteryType.Basic),
                        isLock = true
                    },
                    expertRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(TraditionalRoomMasteryType.Expert),
                        isLock = true
                    },
                    masterRoom = new UITraditionalRoomType.Entity()
                    {
                        btn = SelectMasteryBtnEntity(TraditionalRoomMasteryType.Master),
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

    private ButtonComponent.Entity SelectMasteryBtnEntity(TraditionalRoomMasteryType traditionalRoomMasteryType)
    {
        return new ButtonComponent.Entity(UniTask.Action(async () =>
        {
            HorseRaceContext.RaceMatchDataContext.TraditionalRoomMasteryType = traditionalRoomMasteryType;
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