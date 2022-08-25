using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationBehaviour : StateMachineBehaviour
{
    public string[] animationNames;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.CrossFadeInFixedTime(animationNames.RandomElement(), 0.25f, layerIndex);
    }
}
