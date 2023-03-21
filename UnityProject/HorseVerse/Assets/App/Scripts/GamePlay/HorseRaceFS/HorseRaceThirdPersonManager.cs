using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class HorseRaceThirdPersonManager : MonoBehaviour, IHorseRaceManager
{
    [SerializeField] private CinemachineTargetGroup horseGroup;
    [SerializeField] private Transform horsesContainer;
    private string mapSettingsPath;
    private MapSettings mapSettings;
    private TargetGenerator targetGenerator;
    private bool isStarted;
    public Transform WarmUpTarget => horseGroup.Transform;
    public IHorseRaceInGameStatus[] HorseControllers => HorseRaceThirdPersonBehaviours;
    private HorseRaceThirdPersonBehaviour[] HorseRaceThirdPersonBehaviours { get; set; }
    public float NormalizedRaceTime { get; private set; }
    public int PlayerHorseIndex { get; private set; }
    public event Action OnFinishTrackEvent;
    
    public UniTask WaitToStart()
    {
        throw new NotImplementedException();
    }

    public void PrepareToRace()
    {
        SetHorsesVisible(true);
    }

    public void StartRace()
    {
        isStarted = true;
        HorseRaceThirdPersonBehaviours.ForEach(x => x.StartRace(0.0f));
    }

    public async UniTask InitializeAsync(MasterHorseContainer masterHorseContainer,
                                         string mapSettingPath,
                                         int playerHorseIndex,
                                         HorseRaceThirdPersonInfo[] horseRaceThirdPersonInfo,
                                         CancellationToken token)
    {
        PlayerHorseIndex = playerHorseIndex;
        this.mapSettingsPath = mapSettingPath;
        await LoadMapSettings(token);
        HorseRaceThirdPersonBehaviours = await horseRaceThirdPersonInfo.Select(x => LoadHorseController(masterHorseContainer, x, token));
        SetHorsesVisible(false);
    }

    private void SetHorsesVisible(bool isVisible)
    {
        HorseRaceThirdPersonBehaviours.ForEach(x => gameObject.SetActive(isVisible));
    }

    private async UniTask LoadMapSettings(CancellationToken token)
    {
        mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(mapSettingsPath, token);
        targetGenerator = Instantiate(mapSettings.targetGenerator, Vector3.zero, Quaternion.identity);
    }

    private async UniTask<HorseRaceThirdPersonBehaviour> LoadHorseController(MasterHorseContainer masterHorseContainer, HorseRaceThirdPersonInfo horseRaceThirdPersonInfo, CancellationToken token)
    {
        var horse = await HorseMeshAssetLoader.InstantiateHorse(masterHorseContainer.GetHorseMeshInformation(horseRaceThirdPersonInfo.MeshInformation, HorseModelMode.RaceThirdPerson) , token);
        horse.transform.SetParent(horsesContainer);
        var horseController = horse.GetComponent<HorseRaceThirdPersonBehaviour>();
        horseGroup.AddMember(horseController.transform, 1, 0);
        return horseController;
    }

    private void FixedUpdate()
    {
        if (!isStarted) return;
        NormalizedRaceTime = HorseRaceThirdPersonBehaviours.Min(x => x.CurrentRaceProgressWeight);
    }
    
    public void Dispose()
    {
        Destroy(mapSettings);
        mapSettings = default;
        PrimitiveAssetLoader.UnloadAssetAtPath(mapSettingsPath);
        
        DisposeUtility.SafeDisposeMonoBehaviour(ref targetGenerator);
        Time.timeScale = 1;
    }
}
