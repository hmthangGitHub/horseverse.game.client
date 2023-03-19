using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class HorseRaceManagerFactory : IHorseRaceManagerFactory
{
    private IReadOnlyHorseRepository horseRepository;
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private HorseRaceContext HorseRaceContext => Container.Inject<HorseRaceContext>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= Container.Inject<HorseRepository>();
    private IDIContainer Container { get; }

    public HorseRaceManagerFactory(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask<IHorseRaceManager> CreateHorseRaceManagerAsync(CancellationToken token)
    {
        var horseRaceManager = Object.Instantiate((await Resources.LoadAsync<HorseRaceManager>("GamePlay/HorseRaceManager") as HorseRaceManager));
        var playerHorseIndex = -1;
        if (HorseRaceContext.GameMode == HorseGameMode.Race)
        {
            playerHorseIndex = HorseRaceContext.RaceScriptData.
                                                HorseRaceInfos.
                                                ToList().
                                                FindIndex(x => HorseRepository.Models.ContainsKey(x.NftHorseId));
        }
        
        var masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>(token);
        var masterMap = masterMapContainer.MasterMapIndexer[HorseRaceContext.RaceScriptData.MasterMapId];
        MasterLoader.SafeRelease(ref masterMapContainer);
        
        await horseRaceManager.InitializeAsync(HorseRaceContext.RaceScriptData.HorseRaceInfos.Select(x => MasterHorseContainer.GetHorseMeshInformation(x.MeshInformation, HorseModelMode.Race)).ToArray(),
            masterMap.MapSettings,
            playerHorseIndex,
            HorseRaceContext.RaceScriptData.HorseRaceInfos.Select(x => x.RaceSegments.Sum(segment => segment.Time)).ToArray(),
            HorseRaceContext.RaceScriptData.HorseRaceInfos,
            token);
        return horseRaceManager;
    }
}

public class HorseRaceThirdPersonFactory : IHorseRaceManagerFactory
{
    private IDIContainer Container { get; }

    public HorseRaceThirdPersonFactory(IDIContainer container)
    {
        Container = container;
    }

    public UniTask<IHorseRaceManager> CreateHorseRaceManagerAsync(CancellationToken token)
    {
        return new UniTask<IHorseRaceManager>();
    }
}

public interface IHorseRaceManagerFactory
{
   UniTask<IHorseRaceManager> CreateHorseRaceManagerAsync(CancellationToken token);
}