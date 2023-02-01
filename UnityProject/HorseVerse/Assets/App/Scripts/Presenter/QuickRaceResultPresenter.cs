using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

public class QuickRaceResultPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private UIRaceResultList uiRaceResultList;
    private RaceScriptData raceScriptData;
    private RaceScriptData RaceScriptData => raceScriptData ??= Container.Inject<RaceScriptData>();
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
            horseNames = RaceScriptData.HorseRaceInfos.Select(x => x.Name).ToArray(),
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
        return RaceScriptData.HorseRaceInfos.Select(x =>
                (horseRaceTime: x, totalRaceTime: x.RaceSegments.Sum(raceSegment => raceSegment.Time)))
            .OrderBy(x => x.totalRaceTime)
            .Select((x, i) => new UIComponentHorseResult.Entity()
            {
                isPlayer = x.horseRaceTime.Name == UserDataRepository.Current.UserName,
                lane = i + 1,
                top = i + 1,
                name = x.horseRaceTime.Name,
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