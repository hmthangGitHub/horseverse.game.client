using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public int numberOfAgents;
    public AgentController agentTemplate;
    public float spacing;
    public AgentController[] AgentControllers { get; private set; }

    private void Awake()
    {
        agentTemplate.gameObject.SetActive(false);
    }

    
    public void Spawn()
    {
        var start = agentTemplate.transform.right * (-spacing * ((float)(numberOfAgents - 1) / 2.0f)) + transform.position;
        var offset = agentTemplate.transform.right * spacing;
        var list = new List<AgentController>();
        for (int i = 0; i < numberOfAgents; i++)
        {
            var agentController = GameObject.Instantiate<AgentController>(agentTemplate, start + i * offset, agentTemplate.transform.rotation, this.transform);
            agentController.gameObject.SetActive(true);
            list.Add(agentController);
        }
        AgentControllers = list.ToArray();
    }
}
