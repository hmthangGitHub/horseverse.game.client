using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

public class QuickRaceResultPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private UIRaceResultList uiRaceResultList;
    private RaceMatchData raceMatchData;
    private RaceMatchData RaceMatchData => raceMatchData ??= Container.Inject<RaceMatchData>();
    private IUserDataRepository userDataRepository;
    private IUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IUserDataRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private UIRaceResultSelf uiRaceResultSelf;
    private UIHorseQuickRaceResultList uiHorseQuickRaceResultList;

    private IDIContainer Container { get; }

    public QuickRaceResultPresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask ShowResultAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await ShowRaceResultSelf();
        await ShowResultList();
        await ShowReward();
    }

    private async UniTask ShowResultList()
    {
        var ucs = new UniTaskCompletionSource();
        uiHorseQuickRaceResultList ??= await UILoader.Instantiate<UIHorseQuickRaceResultList>();
        uiHorseQuickRaceResultList.SetEntity(new UIHorseQuickRaceResultList.Entity()
        {
            horseNames = RaceMatchData.horseRaceTimes.Select(x => MasterHorseContainer.MasterHorseIndexer[x.masterHorseId].Name).ToArray(),
            outerBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                await uiHorseQuickRaceResultList.Out();
                ucs.TrySetResult();
            }))
        });
        uiHorseQuickRaceResultList.In().Forget();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    private async UniTask ShowReward()
    {
        uiRaceResultList ??= await UILoader.Instantiate<UIRaceResultList>(token: cts.Token);
        var ucs = new UniTaskCompletionSource();
        uiRaceResultList.SetEntity(new UIRaceResultList.Entity()
        {
            horseList = new UIComponentHorseResultList.Entity()
            {
                entities = GetResultList()
            },
            closeBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            }),
        });

        uiRaceResultList.In().Forget();
        await ucs.Task;
    }

    private async UniTask ShowRaceResultSelf()
    {
        var ucs = new UniTaskCompletionSource();
        uiRaceResultSelf ??= await UILoader.Instantiate<UIRaceResultSelf>();
        RaceMatchData.GetTops();
        var playerIndex = RaceMatchData.horseRaceTimes.ToList().FindIndex(x => x.masterHorseId == UserDataRepository.Current.MasterHorseId);
        var playerTop = RaceMatchData.GetTops()[playerIndex];
        uiRaceResultSelf.SetEntity(new UIRaceResultSelf.Entity()
        {
            name = "My name",
            speech = "The plates will still shift and the clouds will still spew. The sun will slowly rise and the moon will follow too.",
            btnTapAnyWhere = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                await uiRaceResultSelf.Out();
                ucs.TrySetResult();
            })),
            top = playerTop
        });
        uiRaceResultSelf.In().Forget();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    private UIComponentHorseResult.Entity[] GetResultList()
    {
        var top = RaceMatchData.horseRaceTimes.Select(x => x.time).ToArray().GetTopByTimes();
        return RaceMatchData.horseRaceTimes.Select((x , i) => new UIComponentHorseResult.Entity()
        {
            isPlayer = x.masterHorseId == UserDataRepository.Current.MasterHorseId,
            lane = i + 1,
            top = top[i],
            name = MasterHorseContainer.MasterHorseIndexer[x.masterHorseId].Name,
            time = x.time
        }).OrderBy(x => x.top)
          .ToArray();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiRaceResultList);
        UILoader.SafeRelease(ref uiRaceResultSelf);
    }
}