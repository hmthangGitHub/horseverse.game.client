using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTrapBase : MonoBehaviour
{
    public bool IsReady { get; set; }

    public System.Action OnFinishPlatform = ActionUtility.EmptyAction.Instance;

}
