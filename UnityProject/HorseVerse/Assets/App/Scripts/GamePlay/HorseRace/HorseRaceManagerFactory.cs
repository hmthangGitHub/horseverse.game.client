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
            playerHorseIndex = HorseRaceContext.RaceMatchData.
                                                HorseRaceInfos.
                                                ToList().
                                                FindIndex(x => HorseRepository.Models.ContainsKey(x.NftHorseId));
        }
        
        var masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>(token);
        var masterMap = masterMapContainer.MasterMapIndexer[HorseRaceContext.MasterMapId];
        MasterLoader.SafeRelease(ref masterMapContainer);
        
        await horseRaceManager.InitializeAsync(HorseRaceContext.RaceMatchData.HorseRaceInfos.Select(x => MasterHorseContainer.GetHorseMeshInformation(x.MeshInformation, HorseModelMode.Race)).ToArray(),
            masterMap.MapSettings,
            playerHorseIndex,
            HorseRaceContext.RaceMatchData.HorseRaceInfos,
            token);
        return horseRaceManager;
    }
}

public class HorseRaceThirdPersonFactory : IHorseRaceManagerFactory
{
    private IReadOnlyHorseRepository horseRepository;
    private IDIContainer Container { get; }
    private HorseRaceContext HorseRaceContext => Container.Inject<HorseRaceContext>();    
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= Container.Inject<HorseRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();

    public HorseRaceThirdPersonFactory(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask<IHorseRaceManager> CreateHorseRaceManagerAsync(CancellationToken token)
    {
        var horseRaceManager = new HorseRaceThirdPersonManager();
        var playerHorseIndex = -1;
        playerHorseIndex = HorseRaceContext.HorseBriefInfos.
                                            ToList().
                                            FindIndex(x => HorseRepository.Models.ContainsKey(x.NftHorseId));
        
        var masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>(token);
        var masterMap = masterMapContainer.MasterMapIndexer[HorseRaceContext.MasterMapId];
        MasterLoader.SafeRelease(ref masterMapContainer);
        
        await horseRaceManager.InitializeAsync(MasterHorseContainer,
            masterMap.MapSettings,
            playerHorseIndex,
            HorseRaceContext.HorseRaceThirdPersonMatchData.HorseRaceInfos,
            token);
        return horseRaceManager;
    }
}

public interface IHorseRaceManagerFactory
{
   UniTask<IHorseRaceManager> CreateHorseRaceManagerAsync(CancellationToken token);
}