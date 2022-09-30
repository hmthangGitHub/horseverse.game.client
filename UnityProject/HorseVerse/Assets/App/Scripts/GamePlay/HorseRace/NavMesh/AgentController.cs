using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class AgentController : MonoBehaviour
{
    private GameObject target;
    public GameObject Target => target ??= new GameObject($"{gameObject.name}_Target");
    Color Color => color ??= UnityEngine.Random.ColorHSV();

    private int currentTargetIndex = -1;
    Color? color;
    Transform _transform;
    Transform Transform => _transform ?? this.transform;

    public NavMeshAgent navMeshAgent;
    public Vector3[] PredefineTargets { get; set; }

    public bool isStart;
    public event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;

    public void StartRun()
    {
        isStart = true;
        ChangeTarget();
    }


    private void Update()
    {
        if(isStart)
        {
            if (IsReachTarget())
            {
                if(IsLastWayPoint())
                {
                    OnFinishTrackEvent();
                }
                ChangeTarget();
            }
#if UNITY_EDITOR
            DrawDebug();
#endif
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(Application.isPlaying && UnityEditor.Selection.gameObjects.Contains(this.gameObject))
        {
            Gizmos.color = Color;
            PredefineTargets.ForEach(x => Gizmos.DrawSphere(x, 0.5f));
        }
    }

    private void DrawDebug()
    {
        Debug.DrawLine(this.transform.position, Target.transform.position, Color);
    }
#endif

    private bool IsReachTarget()
    {
        return (Transform.position - PredefineTargets[currentTargetIndex]).XZ().magnitude < 0.1f;
    }

    private bool IsLastWayPoint()
    {
        return currentTargetIndex == PredefineTargets.Length - 1;
    }    

    private void ChangeTarget()
    {
        currentTargetIndex++;
        currentTargetIndex %= PredefineTargets.Length;
        navMeshAgent.destination = PredefineTargets[currentTargetIndex];
        Target.transform.position = PredefineTargets[currentTargetIndex];
    }
}
