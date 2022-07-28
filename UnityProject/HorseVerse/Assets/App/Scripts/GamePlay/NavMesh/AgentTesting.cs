using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AgentTesting : MonoBehaviour
{
    public AgentSpawner agentSpawner;
    public TargetSpawner targetSpawner;
    public TargetGenerator targetGenerator;

    private void Start()
    {
        agentSpawner.Spawn();
        targetSpawner.Spawn();

        for (int i = 0; i < agentSpawner.AgentControllers.Length; i++)
        {
            agentSpawner.AgentControllers[i].PredefineTargets = targetGenerator.GenerateRandomTargets();
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            GeneratortestingAsync().Forget();
        }
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        agentSpawner.Spawn();
    }

    [ContextMenu("ChangeTarget")]
    public void ChangeTarget()
    {
        for (int i = 0; i < agentSpawner.AgentControllers.Length; i++)
        {
            agentSpawner.AgentControllers[i].Target.transform.position = agentSpawner.AgentControllers[i].transform.position 
                                                                        + new Vector3(UnityEngine.Random.Range(-1.3f, 1.3f), 0, UnityEngine.Random.Range(20.0f, 20.0f));
        }
    }

    [ContextMenu("Generate Targest")]
    public void GenerateTargets()
    {
        targetGenerator.GenerateRandomTargets();
    }

    public async UniTask GeneratortestingAsync()
    {
        var di = new DIContainer();
        var masterHorse = await MasterLoader.LoadMasterAsync<MasterHorseContainer>();
        di.Bind(masterHorse);
        UserDataRepository dependency = new UserDataRepository();
        await dependency.LoadRepositoryIfNeedAsync();
        di.Bind(dependency);
        var iQuickRaceDomainService = new LocalQuickRaceDomainService(di);
        var result = await iQuickRaceDomainService.FindMatch();
        var targets = targetGenerator.GenerateTargets(result.horseRaceTimes[0].raceSegments);
        for (int i = 0; i < targets.Length; i++)
        {
            var x = targets[i];
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = i.ToString();
            go.transform.position = x.target;
            go.transform.parent = this.transform;
        }
        
    }
}
