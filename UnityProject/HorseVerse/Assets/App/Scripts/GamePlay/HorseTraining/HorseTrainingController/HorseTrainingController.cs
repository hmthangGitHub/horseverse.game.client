using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PathCreation;
using UnityEngine;

public class HorseTrainingController : MonoFSMContainer
{
    [SerializeField] private HorseTrainingAttribute horseTrainingAttribute;
    public HorseTrainingAttribute HorseTrainingAttribute => horseTrainingAttribute;
    [SerializeField] private HorseTrainingControllerData horseTrainingControllerData;
    public HorseTrainingControllerData HorseTrainingControllerData => horseTrainingControllerData;
    

    public PathCreator pathCreator;
    public HorseTrainingTrigger horseTrainingTrigger;
    public Rigidbody rigidbody;
    public new Transform transform;
    public Animator animator;
    public Transform horseMesh;

    public event Action OnTakeCoin = ActionUtility.EmptyAction.Instance;
    public event Action OnTouchObstacle = ActionUtility.EmptyAction.Instance;
    public event Action OnBridgeEvent = ActionUtility.EmptyAction.Instance;
    public event Action OnLandingEvent = ActionUtility.EmptyAction.Instance;
    public event Action OnFallEvent = ActionUtility.EmptyAction.Instance;


    private void Awake()
    {
        SetData(default); // TODO remove
        animator = this.GetComponentInChildren<Animator>(true);
        horseMesh = animator.transform;
        horseTrainingTrigger.OnTouchObstacle += () =>
        {
            ChangeState<HorseTrainingIdleState>();
            OnTouchObstacle.Invoke();
        };
        horseTrainingTrigger.OnTakeCoin += () => OnTakeCoin.Invoke();
        horseTrainingTrigger.OnTouchBridge += ChangeState<HorseTrainingBridgeState>;
    }
    
    public override void AddStates()
    {
        base.AddStates();
        AddState<HorseTrainingCloudState>();        
        AddState<HorseTrainingIdleState>();
        AddState<HorseTrainingBridgeState>();
        AddState<HorseTrainingAirState>();
        AddState<HorseTrainingFallState>();
        SetInitialState<HorseTrainingIdleState>();
    }

    public void SetData(PredefinePath predefinePath)
    {
        transform = base.transform;
    }

    public void StartGame()
    {
        ChangeState<HorseTrainingAirState>();
    }

    public void SetEnablePhysicController(bool enable)
    {
        rigidbody.useGravity = enable;
        rigidbody.isKinematic = !enable;
    }

    public void OnBridge()
    {
        OnBridgeEvent.Invoke();
    }
    
    public void OnLanding()
    {
        OnLandingEvent.Invoke();
    }

    public void OnFall()
    {
        OnFallEvent.Invoke();
    }
    
    
}