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
        await ShowResultList();
        await ShowReward();
    }

    private async UniTask ShowResultList()
    {
        var ucs = new UniTaskCompletionSource();
        uiHorseQuickRaceResultList ??= await UILoader.Instantiate<UIHorseQuickRaceResultList>();
        uiHorseQuickRaceResultList.SetEntity(new UIHorseQuickRaceResultList.Entity()
        {
            horseNames = RaceMatchData.HorseRaceTimes.Select(x => MasterHorseContainer.MasterHorseIndexer[x.masterHorseId].Name).ToArray(),
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

    private UIComponentHorseResult.Entity[] GetResultList()
    {
        return RaceMatchData.HorseRaceTimes.Select(x =>
                (horseRaceTime: x, totalRaceTime: x.raceSegments.Sum(raceSegment => raceSegment.time)))
            .OrderBy(x => x.totalRaceTime)
            .Select((x, i) => new UIComponentHorseResult.Entity()
            {
                isPlayer = x.horseRaceTime.masterHorseId == UserDataRepository.Current.CurrentHorseNftId,
                lane = i + 1,
                top = i + 1,
                name = MasterHorseContainer.MasterHorseIndexer[x.horseRaceTime.masterHorseId].Name,
                time = x.totalRaceTime
            })
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